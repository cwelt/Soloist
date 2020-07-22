namespace CW.Soloist.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedExistingSongsAsPublic : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE Songs SET IsPublic = 1");
        }
        
        public override void Down()
        {
            Sql("UPDATE Songs SET IsPublic = 0");
        }
    }
}
