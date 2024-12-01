using Check_Inn.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Check_Inn.DAL
{
    public class CheckInnSqlServerContext: IdentityDbContext<User>
    {
        public CheckInnSqlServerContext() : base("MySqlConnection") 
        {
        }
        public static CheckInnSqlServerContext Create()
        {
            return new CheckInnSqlServerContext();
        }

        public DbSet<AccomodationType> AccomodationTypes { get; set; }
        public DbSet<AccomodationPackage> AccomodationPackages { get; set; }
        public DbSet<Accomodation> Accomodations { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}