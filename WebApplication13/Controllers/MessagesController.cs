using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml.Serialization;
using WebApplication13.Models;

using RabbitMQ.Client;

namespace WebApplication13.Controllers
{
    public class MessagesController : ApiController
    {
        private WebApplication13Context db = new WebApplication13Context();
        private static readonly string CONNECTION_URI = "amqp://vxoqwope:CQyPw9I5N8Hn70MjvQu0dd9lwcdnJZA0@spotted-monkey.rmq.cloudamqp.com/vxoqwope";
        private static ConnectionFactory factory = new ConnectionFactory();
        private static IConnection rabbitMqConnection = null;


        //// GET: api/Messages
        //public IQueryable<Message> GetMessages()
        //{
        //    return db.Messages;
        //}

        // GET: api/Messages?from={from}&to={to}
        [ResponseType(typeof(List<MessageDTO>))]
        public async Task<IHttpActionResult> GetMessage(String from, String to)
        {
            Debug.WriteLine("from: " + from);
            Debug.WriteLine("to: " + to);

            User userTo = db.Users.Where(b => b.Email == to).Include(b => b.Messages.Select(y => y.Sender)).FirstOrDefault();
            User userFrom = db.Users.Where(b => b.Email == from).Include(b => b.Messages.Select(y => y.Receiver)).FirstOrDefault();

            if (userTo == null)
                return BadRequest("User " + to + " doesn't exist. Please check your spelling.");

            if (userFrom == null)
                return BadRequest("User " + from + " doesn't exist. Please check your spelling.");

            var returnMessages = new List<MessageDTO>();

            foreach (Message msg in userTo.Messages)
            {
                if (!msg.Sender.Email.Equals(from, StringComparison.CurrentCultureIgnoreCase))
                    continue;
                returnMessages.Add(new MessageDTO
                {
                    Id = msg.Id,
                    Email = msg.Sender.Email,
                    Image = msg.Image,
                    Text = msg.Text,
                    Timestamp = msg.Timestamp
                });
            }

            foreach (Message msg in userFrom.Messages)
            {
                if (!msg.Sender.Email.Equals(to, StringComparison.CurrentCultureIgnoreCase))
                    continue;
                returnMessages.Add(new MessageDTO
                {
                    Id = msg.Id,
                    Email = msg.Sender.Email,
                    Image = msg.Image,
                    Text = msg.Text,
                    Timestamp = msg.Timestamp
                });
            }

            Debug.WriteLine("msg count: " + returnMessages.Count());
            returnMessages.Sort(delegate (MessageDTO msg1, MessageDTO msg2) { return msg1.Timestamp.CompareTo(msg2.Timestamp); });

            foreach (MessageDTO msg in returnMessages)
            {
                Debug.WriteLine("id: " + msg.Id + ", timestamp: " + msg.Timestamp);
            }

            return Ok(returnMessages);
        }

        //// PUT: api/Messages/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutMessage(int id, Message message)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != message.Id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(message).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!MessageExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST: api/Messages
        [ResponseType(typeof(MessagePOST))]
        public async Task<IHttpActionResult> PostMessage(MessagePOST messagePost)
        {
            Debug.WriteLine("\nimage: " + messagePost.Image);
            Debug.WriteLine("text: " + messagePost.Text);
            Debug.WriteLine("sender: " + messagePost.Sender);
            Debug.WriteLine("receiver: " + messagePost.Receiver);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User sender = db.Users.Where(b => b.Email == messagePost.Sender).Include(b => b.Messages).FirstOrDefault();
            if (sender == null)
                return BadRequest("User " + messagePost.Sender + " doesn't exist. Please check your spelling.");

            User receiver = db.Users.Where(b => b.Email == messagePost.Receiver).Include(b => b.Messages).FirstOrDefault();
            if (receiver == null)
                return BadRequest("User " + messagePost.Receiver + " doesn't exist. Please check your spelling.");

            var message = new Message();
            message.Email = sender.Email;
            message.Image = messagePost.Image;
            message.Text = messagePost.Text;
            message.Timestamp = DateTime.Now;
            message.Sender = sender;
            message.Receiver = receiver;

            receiver.Messages.Add(message);

            db.Messages.Add(message);

            await db.SaveChangesAsync();

            /*
             * Code below pushes to RabbitMQ queues.
             *
             */

            // Create a new connection to broker if not already created from before.
            if (rabbitMqConnection == null) {
                factory.Uri = CONNECTION_URI;
                rabbitMqConnection = factory.CreateConnection();
            }

            // Create a channel (virtual connection) within the connection
            using (var channel = rabbitMqConnection.CreateModel()) {
                channel.ExchangeDeclare(exchange: "chat", type: "direct");

                // Name of queue, format: {sender@email.com}TO{receiver@email.com}
                string queueName = sender.Email + "TO" + receiver.Email;

                // Declare which queue (create one if it doesn't already exist) to use when publishing message
                channel.QueueDeclare(queueName, true, false, false, null);
                channel.QueueBind(queue: queueName, exchange: "chat", routingKey: queueName);

                // Create object to publish
                MessageDTO msgToPush = new MessageDTO {
                    // need to get message that was saved to db if we want to send the actual message id
                    Id = -1,
                    Email = messagePost.Sender,
                    Image = messagePost.Image,
                    Text = messagePost.Text,
                    Timestamp = DateTime.Now
                };

                // Serialize object and set it as the body of the message to save to queue
                string serializedMessage = Serialize(msgToPush);
                var body = Encoding.UTF8.GetBytes(serializedMessage);

                // Publish to "chat" exchanger with route using queue name to save it to the proper queue
                channel.BasicPublish(exchange: "chat",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);
                Debug.WriteLine("Sent to broker: " + serializedMessage);
            }


            return Ok(messagePost);
        }

        //    // DELETE: api/Messages/5
        //    [ResponseType(typeof(Message))]
        //    public async Task<IHttpActionResult> DeleteMessage(int id)
        //    {
        //        Message message = await db.Messages.FindAsync(id);
        //        if (message == null)
        //        {
        //            return NotFound();
        //        }

        //        db.Messages.Remove(message);
        //        await db.SaveChangesAsync();

        //        return Ok(message);
        //    }

        //    protected override void Dispose(bool disposing)
        //    {
        //        if (disposing)
        //        {
        //            db.Dispose();
        //        }
        //        base.Dispose(disposing);
        //    }

        //    private bool MessageExists(int id)
        //    {
        //        return db.Messages.Count(e => e.Id == id) > 0;
        //    }
        //}

        // http://stackoverflow.com/questions/2434534/serialize-an-object-to-string
        public static string Serialize<T>(T toSerialize) {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
}