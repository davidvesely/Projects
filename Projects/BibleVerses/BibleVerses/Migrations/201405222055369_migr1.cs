namespace BibleVerses.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migr1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BibleVerses", "BiblePlaceID", "dbo.BiblePlaces");
            DropPrimaryKey("dbo.BiblePlaces");
            DropPrimaryKey("dbo.BibleVerses");
            AlterColumn("dbo.BiblePlaces", "ID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.BibleVerses", "ID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.BiblePlaces", "ID");
            AddPrimaryKey("dbo.BibleVerses", "ID");
            AddForeignKey("dbo.BibleVerses", "BiblePlaceID", "dbo.BiblePlaces", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BibleVerses", "BiblePlaceID", "dbo.BiblePlaces");
            DropPrimaryKey("dbo.BibleVerses");
            DropPrimaryKey("dbo.BiblePlaces");
            AlterColumn("dbo.BibleVerses", "ID", c => c.Int(nullable: false));
            AlterColumn("dbo.BiblePlaces", "ID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.BibleVerses", "ID");
            AddPrimaryKey("dbo.BiblePlaces", "ID");
            AddForeignKey("dbo.BibleVerses", "BiblePlaceID", "dbo.BiblePlaces", "ID");
        }
    }
}
