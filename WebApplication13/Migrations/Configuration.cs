namespace WebApplication13.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebApplication13.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WebApplication13.Models.WebApplication13Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "WebApplication13.Models.WebApplication13Context";
        }

        protected override void Seed(WebApplication13.Models.WebApplication13Context context)
        {
            User johan = new User
            {
                Id = 1,
                Lastlogin = DateTime.Now,
                Email = "Peonsson@gmail.com",
                Register = DateTime.Now,
                Status = "Online",
                ImageUrl = "https://s-media-cache-ak0.pinimg.com/736x/35/35/2b/35352b1a98370e68d2000f074a827c20.jpg",
                Friends = new List<User> { },
                Messages = new List<Message> { },
                LastReceivedMessage = DateTime.Now
            };

            User robin = new User
            {
                Id = 2,
                Lastlogin = DateTime.Now,
                Email = "roppe546@gmail.com",
                Register = DateTime.Now,
                Status = "Busy",
                ImageUrl = "https://s-media-cache-ak0.pinimg.com/736x/35/35/2b/35352b1a98370e68d2000f074a827c20.jpg",
                Friends = new List<User> { },
                Messages = new List<Message> { },
                LastReceivedMessage = DateTime.Now
            };

            User isak = new User
            {
                Id = 3,
                Lastlogin = DateTime.Now,
                Email = "Isak@gmail.com",
                Register = DateTime.Now,
                Status = "Away",
                ImageUrl = "https://s-media-cache-ak0.pinimg.com/736x/35/35/2b/35352b1a98370e68d2000f074a827c20.jpg",
                Friends = new List<User> { },
                Messages = new List<Message> { },
                LastReceivedMessage = DateTime.Now
            };

            User axel = new User
            {
                Id = 4,
                Lastlogin = DateTime.Now,
                Email = "Axel@gmail.com",
                Register = DateTime.Now,
                Status = "Offline",
                ImageUrl = "https://s-media-cache-ak0.pinimg.com/736x/35/35/2b/35352b1a98370e68d2000f074a827c20.jpg",
                Friends = new List<User> { },
                Messages = new List<Message> { },
                LastReceivedMessage = DateTime.Now
            };

            johan.Friends.Add(robin);
            johan.Friends.Add(isak);
            johan.Friends.Add(axel);

            robin.Friends.Add(johan);
            robin.Friends.Add(isak);
            robin.Friends.Add(axel);

            context.Users.AddOrUpdate(x => x.Id, johan, isak, robin, axel);

            Message msg1 = new Message { Id = 1, Image = "hello world1", Sender = robin, Receiver = johan, Text = "hello world1", Timestamp = DateTime.Now };

            johan.Messages.Add(msg1);

            Message msg2 = new Message { Email = "isak@gmail.com", Id = 2, Image = "hello world2", Sender = isak, Receiver = johan, Text = "hello world2", Timestamp = DateTime.Now };
            Message msg3 = new Message { Email = "axel@gmail.com", Id = 3, Image = "hello world2", Sender = axel, Receiver = johan, Text = "hello world2", Timestamp = DateTime.Now };
            Message msg4 = new Message { Email = "peonsson@gmail.com", Id = 4, Image = "hello world2", Sender = johan, Receiver = johan, Text = "hello world2", Timestamp = DateTime.Now };
            Message msg5 = new Message { Email = "roppe546@gmail.com", Id = 5, Image = "hello world2", Sender = robin, Receiver = johan, Text = "hello world2", Timestamp = DateTime.Now };

            johan.Messages.Add(msg2);
            johan.Messages.Add(msg3);
            johan.Messages.Add(msg4);
            johan.Messages.Add(msg5);

            context.Users.AddOrUpdate(johan);

            context.Messages.AddOrUpdate(x => x.Id, msg1, msg2);
        }
    }
}
