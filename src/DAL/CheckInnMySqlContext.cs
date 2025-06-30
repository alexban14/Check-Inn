using Check_Inn.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using MySql.Data.EntityFramework;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace Check_Inn.DAL
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class CheckInnMySqlContext: IdentityDbContext<User>, ICheckInnContext
    {
        public CheckInnMySqlContext() : base ("MySqlConnection")
        {
        }

        public static CheckInnMySqlContext Create()
        {
            return new CheckInnMySqlContext();
        }
        public virtual DbSet<AccomodationType> AccomodationTypes { get; set; }
        public virtual DbSet<AccomodationPackage> AccomodationPackages { get; set; }
        public virtual DbSet<Accomodation> Accomodations { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }

        public new int SaveChanges()
        {
            return base.SaveChanges();
        }

        public new IDbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
        {
            return new DbEntityEntryWrapper<TEntity>(base.Entry(entity));
        }
    }
}