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
    public class FriendsController : ApiController
    {
        private WebApplication13Context db = new WebApplication13Context();

        //// GET: api/Friends
        //public IQueryable<Friend> GetFriends()
        //{
        //    return db.Friends;
        //}

        //// GET: api/Friends/5
        //[ResponseType(typeof(Friend))]
        //public async Task<IHttpActionResult> GetFriend(int id)
        //{
        //    Friend friend = await db.Friends.FindAsync(id);
        //    if (friend == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(friend);
        //}

        //// PUT: api/Friends/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutFriend(int id, Friend friend)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != friend.Id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(friend).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FriendExists(id))
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

        // POST: api/Friends
        [ResponseType(typeof(FriendPOST))]
        public async Task<IHttpActionResult> PostFriend(FriendPOST friend)
        {

            Debug.WriteLine("friend from: " + friend.From);
            Debug.WriteLine("friend to: " + friend.To);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User friendFrom = db.Users.Where(b => b.Email == friend.From).FirstOrDefault();
            if (friendFrom == null)
                return BadRequest("friend 'from' doesn't exist. Please check your spelling.");

            User friendTo = db.Users.Where(b => b.Email == friend.To).Include(b => b.Friends).FirstOrDefault();
            if (friendTo == null)
                return BadRequest("friend 'to' doesn't exist. Please check your spelling.");

            friendFrom.Friends.Add(friendTo);
            friendTo.Friends.Add(friendFrom);

            foreach (User usr in friendFrom.Friends)
            {
                Debug.WriteLine("friendFrom Email: " + usr.Email);
            }

            foreach (User usr in friendTo.Friends)
            {
                Debug.WriteLine("friendTo Email: " + usr.Email);
            }

            await db.SaveChangesAsync();
            return Ok(friend);
        }

    //    // DELETE: api/Friends/5
    //    [ResponseType(typeof(Friend))]
    //    public async Task<IHttpActionResult> DeleteFriend(int id)
    //    {
    //        Friend friend = await db.Friends.FindAsync(id);
    //        if (friend == null)
    //        {
    //            return NotFound();
    //        }

    //        db.Friends.Remove(friend);
    //        await db.SaveChangesAsync();

    //        return Ok(friend);
    //    }

    //    protected override void Dispose(bool disposing)
    //    {
    //        if (disposing)
    //        {
    //            db.Dispose();
    //        }
    //        base.Dispose(disposing);
    //    }

    //    private bool FriendExists(int id)
    //    {
    //        return db.Friends.Count(e => e.Id == id) > 0;
    //    }
    //}
    }
}