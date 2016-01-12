using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplication13.Models;

namespace WebApplication13.Controllers
{
    public class MessagesController : ApiController
    {
        private WebApplication13Context db = new WebApplication13Context();

        // GET: api/Messages
        public IQueryable<Message> GetMessages()
        {
            return db.Messages;
        }

        // GET: api/Messages/5
        [ResponseType(typeof(List<MessageDTO>))]
        public async Task<IHttpActionResult> GetMessage(String from, String to)
        {
            Debug.WriteLine("from: " + from);
            Debug.WriteLine("to: " + to);

            User user;
            using (var context = new WebApplication13Context())
            {
                user = context.Users.Where(b => b.Email == to).Include(b => b.Messages.Select(y => y.Sender)).FirstOrDefault();
            }

            var returnMessages = new List<MessageDTO>();

            foreach (Message msg in user.Messages)
            {
                if (msg.Sender.Email != from)
                    continue;
                returnMessages.Add(new MessageDTO
                {
                    Email = msg.Sender.Email,
                    Image = msg.Image,
                    Text = msg.Text,
                    Timestamp = msg.Timestamp
                });
            }

            Debug.WriteLine("got here2");

            return Ok(returnMessages);
        }

        // PUT: api/Messages/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMessage(int id, Message message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != message.Id)
            {
                return BadRequest();
            }

            db.Entry(message).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Messages
        [ResponseType(typeof(MessagePOST))]
        public async Task<IHttpActionResult> PostMessage(MessagePOST messagePost)
        {
            Debug.WriteLine("got here1");

            Debug.WriteLine("image: " + messagePost.Image);
            Debug.WriteLine("receiver: " + messagePost.Receiver);
            Debug.WriteLine("sender: " + messagePost.Sender);
            Debug.WriteLine("text: " + messagePost.Text);

            if (!ModelState.IsValid)
            {
                Debug.WriteLine("got here2");
                return BadRequest(ModelState);
            }
            Debug.WriteLine("got here3");
            User sender = db.Users.Find(messagePost.Sender);
            User receiver = db.Users.Find(messagePost.Receiver);
            var message = new Message();
            message.Email = sender.Email;
            message.Image = messagePost.Image;
            message.Text = messagePost.Text;
            message.Timestamp = DateTime.Now;

            Debug.WriteLine("got here4");


            //using (var context = new WebApplication13Context())
            //{
            //    receiver = context.Users.Where(b => b.Email == messagePost.Receiver).Include(b => b.Messages).FirstOrDefault();
            //}

            message.Sender = sender;
            message.Receiver = receiver;

            Debug.WriteLine("got here5");

            receiver.Messages = new List<Message>();
            receiver.Messages.Add(message);
            Debug.WriteLine("got here6");

            db.Messages.Add(message);
            Debug.WriteLine("got here7");

            await db.SaveChangesAsync();
            Debug.WriteLine("got here8");

            using (var context = new WebApplication13Context())
            {
                User user = context.Users.Where(b => b.Email == messagePost.Receiver).Include(b => b.Messages).FirstOrDefault();
                Debug.WriteLine("got here9");
                foreach (Message msg in user.Messages)
                {
                    Debug.WriteLine("got here10");
                    Debug.WriteLine(msg.Id);
                }
                Debug.WriteLine("got here11");
            }

            return Ok(messagePost);
        }

        // DELETE: api/Messages/5
        [ResponseType(typeof(Message))]
        public async Task<IHttpActionResult> DeleteMessage(int id)
        {
            Message message = await db.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            db.Messages.Remove(message);
            await db.SaveChangesAsync();

            return Ok(message);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MessageExists(int id)
        {
            return db.Messages.Count(e => e.Id == id) > 0;
        }
    }
}