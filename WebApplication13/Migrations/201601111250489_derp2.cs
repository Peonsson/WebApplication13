namespace WebApplication13.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class derp2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "ImageUrl", c => c.String());
            AddColumn("dbo.Users", "LastReceivedMessage", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "LastReceivedMessage");
            DropColumn("dbo.Users", "ImageUrl");
        }
    }
}
