using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebApplication13.Models;

namespace WebApplication13.Controllers
{
    public class UploadController : ApiController
    {

        private WebApplication13Context db = new WebApplication13Context();

        // POST: api/Upload
        public async Task<HttpResponseMessage> PostFormData()
        {
            //// Check if the request contains multipart/form-data.
            //if (!Request.Content.IsMimeMultipartContent())
            //{
            //    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            //}

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

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
                sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddDays(180); // Accessible 180 days from adding
                sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;

                // Generate the shared access signature on the blob, setting the constraints directly on the signature.
                string sasBlobToken = blockBlob.GetSharedAccessSignature(sasConstraints);

                // Return the URI string for the container, including the SAS token.
                var downloadUrl = blockBlob.Uri + sasBlobToken;

                Debug.WriteLine(downloadUrl);

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
    }
}
