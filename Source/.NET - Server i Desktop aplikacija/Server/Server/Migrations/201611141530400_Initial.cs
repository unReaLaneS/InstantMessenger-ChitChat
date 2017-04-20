namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Friendships",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Userid = c.Int(nullable: false),
                        Friendid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AppUser", t => t.Userid, cascadeDelete: true)
                .ForeignKey("dbo.AppUser", t => t.Friendid, cascadeDelete: true)
                .Index(t => t.Userid)
                .Index(t => t.Friendid);
            
            CreateTable(
                "dbo.AppUser",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(maxLength: 24, storeType: "nvarchar"),
                        Password = c.String(unicode: false),
                        DateOfBirth = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.UserName, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Friendships", "Friendid", "dbo.AppUser");
            DropForeignKey("dbo.Friendships", "Userid", "dbo.AppUser");
            DropIndex("dbo.AppUser", new[] { "UserName" });
            DropIndex("dbo.Friendships", new[] { "Friendid" });
            DropIndex("dbo.Friendships", new[] { "Userid" });
            DropTable("dbo.AppUser");
            DropTable("dbo.Friendships");
        }
    }
}
