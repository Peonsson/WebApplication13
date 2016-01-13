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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Debug.WriteLine("friend from: " + friend.From);
            Debug.WriteLine("friend to: " + friend.To);

            Debug.WriteLine("got here0");

            User friendFrom = db.Users.Where(b => b.Email == friend.From).FirstOrDefault();
            Debug.WriteLine("got here1");
            if (friendFrom == null)
                return BadRequest("friend 'from' doesn't exist. Check your spelling.");

            User friendTo = db.Users.Where(b => b.Email == friend.To).Include(b => b.Friends).FirstOrDefault();
            Debug.WriteLine("got here2");
            if (friendTo == null)
                return BadRequest("friend 'to' doesn't exist. Check your spelling.");

            Debug.WriteLine("got here3");
            friendFrom.Friends.Add(friendTo);
            Debug.WriteLine("got here4");
            friendTo.Friends.Add(friendFrom);
            Debug.WriteLine("got here5");

            foreach (User usr in friendFrom.Friends)
            {
                Debug.WriteLine("got here6");
                Debug.WriteLine("friendFrom Email: " + usr.Email);
            }

            foreach (User usr in friendTo.Friends)
            {
                Debug.WriteLine("got here7");
                Debug.WriteLine("friendTo Email: " + usr.Email);
            }

            Debug.WriteLine("got here8");
            await db.SaveChangesAsync();
            Debug.WriteLine("got here9");
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