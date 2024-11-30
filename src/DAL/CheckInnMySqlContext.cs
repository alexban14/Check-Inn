using Check_Inn.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using MySql.Data.EntityFramework;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Check_Inn.DAL
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class CheckInnMySqlContext: IdentityDbContext<User>
    {
        public CheckInnMySqlContext() : base ("MySqlConnection")
        {
        }

        public static CheckInnMySqlContext Create()
        {
            return new CheckInnMySqlContext();
        }
        public DbSet<AccomodationType> AccomodationTypes { get; set; }
        public DbSet<AccomodationPackage> AccomodationPackages { get; set; }
        public DbSet<Accomodation> Accomodations { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}