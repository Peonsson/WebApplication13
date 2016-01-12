namespace WebApplication13.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class derp9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "Email");
        }
    }
}
