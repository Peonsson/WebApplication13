﻿using System.Data.Entity;

namespace WebApplication13.Models
{
    public class WebApplication13Context : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public WebApplication13Context() : base("name=WebApplication13Context")
        {
        }

        public System.Data.Entity.DbSet<WebApplication13.Models.User> Users { get; set; }

        public System.Data.Entity.DbSet<WebApplication13.Models.Message> Messages { get; set; }
    }
}