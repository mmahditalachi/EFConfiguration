using System;
using System.Threading;

namespace EFConfiguration
{
    public class EntityChangeObserver
    {
        public event EventHandler<EntityChangedEntryEventArgs> Changed;

        public void OnChanged(EntityChangedEntryEventArgs e)
        {
            ThreadPool.QueueUserWorkItem((_) => Changed?.Invoke(this, e));
        }

        #region singleton

        private static readonly Lazy<EntityChangeObserver> lazy = new Lazy<EntityChangeObserver>(() => new EntityChangeObserver());

        private EntityChangeObserver() { }

        public static EntityChangeObserver Instance => lazy.Value;

        #endregion singleton
    }
}
