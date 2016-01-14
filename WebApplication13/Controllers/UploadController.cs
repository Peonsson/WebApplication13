using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Configuration;

using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using WebApplication13.Models;

namespace WebApplication13.Controllers
{
    public class UploadController : ApiController
    {

        private WebApplication13Context db = new WebApplication13Context();

        // GET: api/Upload
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Upload/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Upload
        public async Task<HttpResponseMessage> PostFormData()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            var email = "";

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // Show all the key-value pairs.
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        Trace.WriteLine(string.Format("{0}: {1}", key, val));
                        if (key.Equals("email", StringComparison.CurrentCultureIgnoreCase))
                            email = val;
                        else
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, key + " is not a valid parameter. Please use parameter email.");
                    }
                }

                MultipartFileData file = provider.FileData.First();
                Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                Trace.WriteLine("Server file path: " + file.LocalFileName);

                // Retrieve storage account from connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

                // Create a container and set permissions if it doesn't exist.
                container.CreateIfNotExists();
                container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                // Retrieve reference to a blob
                string blobName = Guid.NewGuid().ToString();
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

                var fileStream = System.IO.File.OpenRead(file.LocalFileName);
                blockBlob.UploadFromStream(fileStream);

                SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
                sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5);
                sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddDays(30); //accessible 30 days from adding
                sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;

                // Generate the shared access signature on the blob, setting the constraints directly on the signature.
                string sasBlobToken = blockBlob.GetSharedAccessSignature(sasConstraints);

                // Return the URI string for the container, including the SAS token.
                var downloadUrl = blockBlob.Uri + sasBlobToken;

                Debug.WriteLine(downloadUrl);

                User user = db.Users.Where(a => a.Email == email).FirstOrDefault();
                if (user == null)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User " + email + " doesn't exist. Please check your spelling.");

                user.ImageUrl = downloadUrl;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                // Close stream and remove data from disk.
                fileStream.Close();
                System.IO.File.Delete(file.LocalFileName);

                return Request.CreateResponse(HttpStatusCode.OK, downloadUrl);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        //public void Post([FromBody]string value)
        //{
        //}

        // PUT: api/Upload/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Upload/5
        public void Delete(int id)
        {
        }
    }
}
