namespace Check_Inn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedMediaEntity : DbMigration
    {
        public override void Up()
        { 
            CreateTable(
                "dbo.AccomodationPackageMedia",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AccomodationPackageID = c.Int(nullable: false),
                        MediaID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Media", t => t.MediaID, cascadeDelete: true)
                .ForeignKey("dbo.AccomodationPackages", t => t.AccomodationPackageID, cascadeDelete: true)
                .Index(t => t.AccomodationPackageID)
                .Index(t => t.MediaID);
            
            CreateTable(
                "dbo.Media",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UrlPath = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AccomodationMedia",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AccomodationID = c.Int(nullable: false),
                        MediaID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Media", t => t.MediaID, cascadeDelete: true)
                .ForeignKey("dbo.Accomodations", t => t.AccomodationID, cascadeDelete: true)
                .Index(t => t.AccomodationID)
                .Index(t => t.MediaID);
        }
        
        public override void Down()
        { 
            DropForeignKey("dbo.AccomodationMedia", "AccomodationID", "dbo.Accomodations");
            DropForeignKey("dbo.AccomodationMedia", "MediaID", "dbo.Media");
            DropForeignKey("dbo.AccomodationPackageMedia", "AccomodationPackageID", "dbo.AccomodationPackages");
            DropForeignKey("dbo.AccomodationPackagePictures", "MediaID", "dbo.Media");
            DropIndex("dbo.AccomodationMedia", new[] { "MediaID" });
            DropIndex("dbo.AccomodationMedia", new[] { "AccomodationID" });
            DropIndex("dbo.AccomodationPackageMedia", new[] { "MediaID" });
            DropIndex("dbo.AccomodationPackageMedia", new[] { "AccomodationPackageID" });
            DropTable("dbo.AccomodationMedia");
            DropTable("dbo.Media");
            DropTable("dbo.AccomodationPackageMedia");
        }
    }
}
