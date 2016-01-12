﻿using System;
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
            Debug.WriteLine("\nimage: " + messagePost.Image);
            Debug.WriteLine("receiver: " + messagePost.Receiver);
            Debug.WriteLine("sender: " + messagePost.Sender);
            Debug.WriteLine("text: " + messagePost.Text);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User sender = db.Users.Where(b => b.Email == messagePost.Sender).Include(b => b.Messages).FirstOrDefault();
            User receiver = db.Users.Where(b => b.Email == messagePost.Receiver).Include(b => b.Messages).FirstOrDefault();

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

            using (var context = new WebApplication13Context())
            {
                User user = context.Users.Where(b => b.Email == messagePost.Receiver).Include(y => y.Messages).FirstOrDefault();
                if (user == null)
                    Debug.WriteLine("user is null!");

                Debug.WriteLine("user message count: " + user.Messages.Count());
                foreach (Message msg in user.Messages)
                {
                    Debug.WriteLine("msg id: " + msg.Id);
                }
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