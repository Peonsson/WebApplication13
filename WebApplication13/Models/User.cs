using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication13.Models
{
    public class User
    {
        [Key]
        [Required]
        public String Email { get; set; }
        public DateTime Lastlogin { get; set; }
        public DateTime Register { get; set; }
        public String Status { get; set; }
        public String ImageUrl { get; set; }
        public DateTime LastReceivedMessage { get; set; }

        public ICollection<Message> Messages { get; set; }
        public ICollection<User> Friends { get; set; }
    }
}