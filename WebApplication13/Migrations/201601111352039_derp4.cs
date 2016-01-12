namespace WebApplication13.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class derp4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Messages", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Messages", "Receiver_Id", "dbo.Users");
            DropForeignKey("dbo.Messages", "Sender_Id", "dbo.Users");
            DropIndex("dbo.Messages", new[] { "User_Id" });
            DropIndex("dbo.Messages", new[] { "Receiver_Id" });
            DropIndex("dbo.Messages", new[] { "Sender_Id" });
            DropIndex("dbo.Users", new[] { "User_Id" });
            RenameColumn(table: "dbo.Messages", name: "Receiver_Id", newName: "Receiver_Email");
            RenameColumn(table: "dbo.Messages", name: "Sender_Id", newName: "Sender_Email");
            RenameColumn(table: "dbo.Users", name: "User_Id", newName: "User_Email");
            RenameColumn(table: "dbo.Messages", name: "User_Id", newName: "User_Email");
            DropPrimaryKey("dbo.Users");
            AlterColumn("dbo.Messages", "User_Email", c => c.String(maxLength: 128));
            AlterColumn("dbo.Messages", "Receiver_Email", c => c.String(maxLength: 128));
            AlterColumn("dbo.Messages", "Sender_Email", c => c.String(maxLength: 128));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Users", "User_Email", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.Users", "Email");
            CreateIndex("dbo.Messages", "User_Email");
            CreateIndex("dbo.Messages", "Receiver_Email");
            CreateIndex("dbo.Messages", "Sender_Email");
            CreateIndex("dbo.Users", "User_Email");
            AddForeignKey("dbo.Users", "User_Email", "dbo.Users", "Email");
            AddForeignKey("dbo.Messages", "User_Email", "dbo.Users", "Email");
            AddForeignKey("dbo.Messages", "Receiver_Email", "dbo.Users", "Email");
            AddForeignKey("dbo.Messages", "Sender_Email", "dbo.Users", "Email");
            DropColumn("dbo.Users", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Id", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.Messages", "Sender_Email", "dbo.Users");
            DropForeignKey("dbo.Messages", "Receiver_Email", "dbo.Users");
            DropForeignKey("dbo.Messages", "User_Email", "dbo.Users");
            DropForeignKey("dbo.Users", "User_Email", "dbo.Users");
            DropIndex("dbo.Users", new[] { "User_Email" });
            DropIndex("dbo.Messages", new[] { "Sender_Email" });
            DropIndex("dbo.Messages", new[] { "Receiver_Email" });
            DropIndex("dbo.Messages", new[] { "User_Email" });
            DropPrimaryKey("dbo.Users");
            AlterColumn("dbo.Users", "User_Email", c => c.Int());
            AlterColumn("dbo.Users", "Email", c => c.String());
            AlterColumn("dbo.Messages", "Sender_Email", c => c.Int());
            AlterColumn("dbo.Messages", "Receiver_Email", c => c.Int());
            AlterColumn("dbo.Messages", "User_Email", c => c.Int());
            AddPrimaryKey("dbo.Users", "Id");
            RenameColumn(table: "dbo.Messages", name: "User_Email", newName: "User_Id");
            RenameColumn(table: "dbo.Users", name: "User_Email", newName: "User_Id");
            RenameColumn(table: "dbo.Messages", name: "Sender_Email", newName: "Sender_Id");
            RenameColumn(table: "dbo.Messages", name: "Receiver_Email", newName: "Receiver_Id");
            CreateIndex("dbo.Users", "User_Id");
            CreateIndex("dbo.Messages", "Sender_Id");
            CreateIndex("dbo.Messages", "Receiver_Id");
            CreateIndex("dbo.Messages", "User_Id");
            AddForeignKey("dbo.Messages", "Sender_Id", "dbo.Users", "Id");
            AddForeignKey("dbo.Messages", "Receiver_Id", "dbo.Users", "Id");
            AddForeignKey("dbo.Messages", "User_Id", "dbo.Users", "Id");
            AddForeignKey("dbo.Users", "User_Id", "dbo.Users", "Id");
        }
    }
}
