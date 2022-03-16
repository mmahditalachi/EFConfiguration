using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EFConfiguration
{
    public class EFConfigurationContext : DbContext
    {
        public EFConfigurationContext(DbContextOptions<EFConfigurationContext> options) : base(options)
        {
        }

        public DbSet<ConfigurationSetting> ConfigurationSettings { get; set; }

        public override int SaveChanges()
        {
            OnEntityChange();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            OnEntityChange();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void OnEntityChange()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(i => i.State == EntityState.Modified || i.State == EntityState.Added))
            {
                EntityChangeObserver.Instance.OnChanged(new EntityChangedEntryEventArgs(entry));
            }
        }
    }

    public class EntityChangedEntryEventArgs : EventArgs
    {
        public EntityEntry Entry { get; set; }

        public EntityChangedEntryEventArgs(EntityEntry entityEntry)
        {
            Entry = entityEntry;
        }
    }
}
