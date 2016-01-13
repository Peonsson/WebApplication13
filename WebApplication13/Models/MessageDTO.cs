using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication13.Models
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public String Email { get; set; }
        public String Image { get; set; }
        public String Text { get; set; }
        public DateTime Timestamp { get; set; }
    }
}