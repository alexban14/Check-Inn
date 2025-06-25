using Check_Inn.Entities;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Check_Inn.DAL
{
    public interface ICheckInnContext
    {
        DbSet<AccomodationType> AccomodationTypes { get; }
        DbSet<AccomodationPackage> AccomodationPackages { get; }
        DbSet<Accomodation> Accomodations { get; }
        DbSet<Booking> Bookings { get; }
        DbSet<Payment> Payments { get; }
        
        int SaveChanges();
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        void Dispose();
    }
}
