namespace CW.Soloist.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMelodyTrackIndexToSongTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Songs", "MelodyTrackIndex", c => c.Int(nullable: true));
            Sql("UPDATE Songs SET MelodyTrackIndex = 1");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Songs", "MelodyTrackIndex");
        }
    }
}
