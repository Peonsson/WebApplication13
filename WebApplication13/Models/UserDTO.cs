using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication13.Models
{
    public class UserDTO
    {
        public String Email { get; set; }
        public DateTime Lastlogin { get; set; }
        public DateTime Register { get; set; }
        public String Status { get; set; }
        public String ImageUrl { get; set; }

        public ICollection<FriendDTO> Friends { get; set; }
    }
}