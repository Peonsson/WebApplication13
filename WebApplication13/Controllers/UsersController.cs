using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplication13.Models;

namespace WebApplication13.Controllers
{
    public class UsersController : ApiController
    {
        private WebApplication13Context db = new WebApplication13Context();

        // GET: api/Users
        public IQueryable<UserDTO> GetUsers()
        {
            var users = from u in db.Users
                        select new UserDTO()
                        {
                            Email = u.Email,
                            Lastlogin = u.Lastlogin,
                            Register = u.Register,
                            Status = u.Status,
                            ImageUrl = u.ImageUrl,
                            Friends = u.Friends.Select(m => new FriendDTO
                            {
                                ImageUrl = u.ImageUrl,
                                Email = m.Email,
                            }).ToList()
                        };

            return users;
        }

        // GET: api/Users?email={email}
        [ResponseType(typeof(UserDTO))]
        public async Task<IHttpActionResult> GetUser(String email)
        {
            var user = await db.Users.Include(u => u.Friends).Select(u => new UserDTO()
            {
                Email = u.Email,
                Lastlogin = u.Lastlogin,
                Register = u.Register,
                Status = u.Status,
                ImageUrl = u.ImageUrl,
                Friends = u.Friends.Select(m => new FriendDTO
                {
                    Email = m.Email,
                    ImageUrl = m.ImageUrl,
                    LastReceivedMessage = m.LastReceivedMessage,
                }).ToList()
            }).SingleOrDefaultAsync(u => u.Email.Equals(email));

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        //TODO: add friends controller
        //TODO: google API for adding images to new users
        //TODO: web-chat client

        //// PUT: api/Users/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutUser(int id, User user)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != user.Id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(user).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UserExists(id))
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

        // POST: api/Users
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(UserPOST userPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = new Models.User();
            user.Email = userPost.Email;
            user.Lastlogin = DateTime.Now;
            user.Register = DateTime.Now;
            user.Status = "Online";
            user.ImageUrl = userPost.ImageUrl;
            user.LastReceivedMessage = DateTime.Now;

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Ok(userPost);
        }

        //        // DELETE: api/Users/5
        //        [ResponseType(typeof(User))]
        //        public async Task<IHttpActionResult> DeleteUser(int id)
        //        {
        //            User user = await db.Users.FindAsync(id);
        //            if (user == null)
        //            {
        //                return NotFound();
        //            }

        //            db.Users.Remove(user);
        //            await db.SaveChangesAsync();

        //            return Ok(user);
        //        }

        //        protected override void Dispose(bool disposing)
        //        {
        //            if (disposing)
        //            {
        //                db.Dispose();
        //            }
        //            base.Dispose(disposing);
        //        }

        //        private bool UserExists(int id)
        //        {
        //            return db.Users.Count(e => e.Id == id) > 0;
        //        }
    }
}