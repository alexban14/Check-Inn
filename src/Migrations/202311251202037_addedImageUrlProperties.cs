namespace Check_Inn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedImageUrlProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccomodationTypes", "Image", c => c.String());
            AddColumn("dbo.Accomodations", "Image", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accomodations", "Image");
            DropColumn("dbo.AccomodationTypes", "Image");
        }
    }
}
