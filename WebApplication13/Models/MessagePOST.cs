using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApplication13.Models
{
    public class MessagePOST
    {
        public String Image { get; set; }
        public String Text { get; set; }
        public String Sender { get; set; }
        public String Receiver { get; set; }
    }
}