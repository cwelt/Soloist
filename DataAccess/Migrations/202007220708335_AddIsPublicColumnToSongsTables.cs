namespace CW.Soloist.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsPublicColumnToSongsTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Songs", "IsPublic", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Songs", "IsPublic");
        }
    }
}
