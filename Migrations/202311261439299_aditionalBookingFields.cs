namespace Check_Inn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aditionalBookingFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "NoOfAdults", c => c.Int(nullable: false));
            AddColumn("dbo.Bookings", "NoOfChildren", c => c.Int(nullable: false));
            AddColumn("dbo.Bookings", "GuestName", c => c.String());
            AddColumn("dbo.Bookings", "Email", c => c.String());
            AddColumn("dbo.Bookings", "AdditionalInfo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bookings", "AdditionalInfo");
            DropColumn("dbo.Bookings", "Email");
            DropColumn("dbo.Bookings", "GuestName");
            DropColumn("dbo.Bookings", "NoOfChildren");
            DropColumn("dbo.Bookings", "NoOfAdults");
        }
    }
}
