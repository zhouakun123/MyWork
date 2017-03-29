using NinjaDomain.Classes;
using NinjaDomain.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjiaDomain.DataModel
{
    public class NinjaContext : DbContext
    {
        public DbSet<Ninja> Ninjas { get; set; }
        public DbSet<Clan> Clans { get; set; }
        public DbSet<NinjaEquipment> Equipment { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Types().Configure(c => c.Ignore("IsDirty"));
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            foreach (var history in this.ChangeTracker.Entries().Where(e => e.Entity is IMotificationHistory && 
            (e.State == EntityState.Added || e.State == EntityState.Modified)).
                Select(e => e.Entity as IMotificationHistory))
            {
                history.DateModified = DateTime.Now;
                if(history.DateCreated == DateTime.MinValue)
                {
                    history.DateCreated = DateTime.Now;
                }
            }
            int result = base.SaveChanges();
            foreach(var history in this.ChangeTracker.Entries()
                .Where(e=>e.Entity is IMotificationHistory)
                .Select(e=>e.Entity as IMotificationHistory))
            {
                history.IsDirty = false;
            }
            return result;
        }
    }
}
