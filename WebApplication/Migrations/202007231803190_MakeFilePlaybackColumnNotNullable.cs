namespace CW.Soloist.WebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeFilePlaybackColumnNotNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Songs", "MidiPlaybackFileName", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Songs", "MidiPlaybackFileName", c => c.String(maxLength: 100));
        }
    }
}
