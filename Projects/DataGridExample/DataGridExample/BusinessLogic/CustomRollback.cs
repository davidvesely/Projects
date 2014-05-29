using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DataGridExample.DatabaseModel;

namespace DataGridExample.BusinessLogic
{
    public static class CustomRollback
    {
        public static void Rollback(this HREntities db)
        {
            var changedEntries = db.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged).ToList();

            foreach (var entry in changedEntries.Where(x => x.State == EntityState.Modified))
            {
                entry.CurrentValues.SetValues(entry.OriginalValues);
                entry.State = EntityState.Unchanged;
            }

            foreach (var entry in changedEntries.Where(x => x.State == EntityState.Added))
            {
                entry.State = EntityState.Detached;
            }

            foreach (var entry in changedEntries.Where(x => x.State == EntityState.Deleted))
            {
                entry.State = EntityState.Unchanged;
            }

        }
    }
}
