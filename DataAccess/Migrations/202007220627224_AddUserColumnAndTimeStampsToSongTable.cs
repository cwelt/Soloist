namespace CW.Soloist.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserColumnAndTimeStampsToSongTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Songs", "Created", c => c.DateTime(nullable: false));
            AddColumn("dbo.Songs", "Modified", c => c.DateTime(nullable: false));
            AddColumn("dbo.Songs", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Songs", "Artist", c => c.String(maxLength: 50));
            CreateIndex("dbo.Songs", "UserId");
            
            // Populdate DB with Roles and admin user    
            Sql("INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'e8ee0172-42fd-4525-a6d0-ddda88584719', N'Admin')");
            Sql("INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'd677de85-63c4-4bf2-ab5d-b8d32e5297f1', N'ApplicationUser')");
            Sql("INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'27e8799a-8dfc-4f6a-b809-3f33f948a841', N'soloist20586project@gmail.com', 0, N'AOobnq8G2Y/NM6wAJXABl+pBjkTnZEMDij5taJQ8h8VJWRozz7z89qxqe+VWH7HLQg==', N'8fea09fb-61a7-4fe7-bee0-35576a81eb19', NULL, 0, 0, NULL, 1, 0, N'soloist20586project@gmail.com')");
            Sql("INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'27e8799a-8dfc-4f6a-b809-3f33f948a841', N'e8ee0172-42fd-4525-a6d0-ddda88584719')");

            // Update current songs in DB with admin user 
            Sql("UPDATE Songs SET UserId = '27e8799a-8dfc-4f6a-b809-3f33f948a841'");

            AddForeignKey("dbo.Songs", "UserId", "dbo.AspNetUsers", "Id");
        }

    public override void Down()
        {
            DropForeignKey("dbo.Songs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Songs", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            AlterColumn("dbo.Songs", "Artist", c => c.String());
            DropColumn("dbo.Songs", "UserId");
            DropColumn("dbo.Songs", "Modified");
            DropColumn("dbo.Songs", "Created");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
        }
    }
}
