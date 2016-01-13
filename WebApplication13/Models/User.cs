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
        public int Id { get; set; }
        public String Email { get; set; }
        public DateTime Lastlogin { get; set; }
        public DateTime Register { get; set; }
        public String Status { get; set; }
        public String ImageUrl { get; set; }
        public DateTime LastReceivedMessage { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        [InverseProperty("Friends")]
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<User> Friends { get; set; }
    }
}