using System.Data.Entity;

namespace Check_Inn.DAL
{
    public interface IDbEntityEntry<TEntity> where TEntity : class
    {
        EntityState State { get; set; }
        TEntity Entity { get; }
    }

    public class DbEntityEntryWrapper<TEntity> : IDbEntityEntry<TEntity> where TEntity : class
    {
        private readonly System.Data.Entity.Infrastructure.DbEntityEntry<TEntity> _entry;

        public DbEntityEntryWrapper(System.Data.Entity.Infrastructure.DbEntityEntry<TEntity> entry)
        {
            _entry = entry;
        }

        public EntityState State
        {
            get => _entry.State;
            set => _entry.State = value;
        }

        public TEntity Entity => _entry.Entity;
    }
}
