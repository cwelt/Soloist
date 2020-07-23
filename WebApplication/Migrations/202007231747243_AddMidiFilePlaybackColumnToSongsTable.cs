namespace CW.Soloist.WebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMidiFilePlaybackColumnToSongsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Songs", "MidiPlaybackFileName", c => c.String(maxLength: 100));
            AlterColumn("dbo.Songs", "Artist", c => c.String(maxLength: 50));
            AlterColumn("dbo.Songs", "MidiFileName", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Songs", "ChordsFileName", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Songs", "ChordsFileName", c => c.String(nullable: false, maxLength: 2000));
            AlterColumn("dbo.Songs", "MidiFileName", c => c.String(nullable: false, maxLength: 2000));
            AlterColumn("dbo.Songs", "Artist", c => c.String());
            DropColumn("dbo.Songs", "MidiPlaybackFileName");
        }
    }
}
