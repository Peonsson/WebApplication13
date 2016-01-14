using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
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
                                Status = m.Status,
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
                    Status = m.Status,
                }).ToList()
            }).SingleOrDefaultAsync(u => u.Email.Equals(email));

            if (user == null)
            {
                return BadRequest("User " + email + " doesn't exist. Please check your spelling.");
            }

            return Ok(user);
        }

        //TODO: add friends controller
        //TODO: google API for adding images to new users
        //TODO: web-chat client

        // PUT: api/Users
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(UserPUT userPut)
        {
            Debug.WriteLine("\nUserPUT email: " + userPut.Email);
            Debug.WriteLine("UserPUT imageUrl: " + userPut.ImageUrl);
            Debug.WriteLine("UserPUT status: " + userPut.Status);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = db.Users.Where(b => b.Email == userPut.Email).FirstOrDefault();
            if (user == null)
                return BadRequest("User " + userPut.Email + " doesn't exist. Please check your spelling.");
            
            // if user didn't change imageUrl she leaves the string empty and we don't change it in database.
            if(!userPut.ImageUrl.Equals(""))
                user.ImageUrl = userPut.ImageUrl;

            // if user didn't change status she leaves the string empty and we don't change it in database.
            if (!userPut.Status.Equals(""))
                user.Status = userPut.Status;

            // if user comes online update lastLogin
            if(userPut.Status.Equals("Online"))
                user.Lastlogin = DateTime.Now;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(userPut);
        }

        // POST: api/Users
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(UserPOST userPost)
        {
            Debug.WriteLine("\nUserPOST email: " + userPost.Email);
            Debug.WriteLine("UserPOST imageUrl: " + userPost.ImageUrl);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User usr = db.Users.Where(b => b.Email == userPost.Email).FirstOrDefault();
            if (usr == null)
            {
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