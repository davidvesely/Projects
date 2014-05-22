namespace BibleVerses.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BiblePlaces",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        BookBG = c.String(),
                        Chapter = c.Int(nullable: false),
                        Start = c.Int(nullable: false),
                        End = c.Int(nullable: false),
                        FullLocation = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.BibleVerses",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Text = c.String(),
                        BiblePlaceID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.BiblePlaces", t => t.BiblePlaceID)
                .Index(t => t.BiblePlaceID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BibleVerses", "BiblePlaceID", "dbo.BiblePlaces");
            DropIndex("dbo.BibleVerses", new[] { "BiblePlaceID" });
            DropTable("dbo.BibleVerses");
            DropTable("dbo.BiblePlaces");
        }
    }
}
