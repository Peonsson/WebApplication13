using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplication13.Models;

namespace WebApplication13.Controllers
{
    public class FriendsController : ApiController
    {
        private WebApplication13Context db = new WebApplication13Context();

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
    }
}