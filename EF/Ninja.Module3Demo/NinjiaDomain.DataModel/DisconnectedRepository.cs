using NinjaDomain.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Collections;

namespace NinjiaDomain.DataModel
{
    public class DisconnectedRepository
    {
        public List<Ninja> GetNinjasWithClan()
        {
            using (var context = new NinjaContext())
            {
                //return context.Ninjas.Include(n=>n.Clan).ToList();
                // Performance with AsNoTracking() method.
                return context.Ninjas.AsNoTracking().Include(n => n.Clan).ToList();
            }
        }

        public Ninja GetNinjaWithEquipment(int id)
        {
            using(var context = new NinjaContext())
            {
                return context.Ninjas.AsNoTracking().Include(n => n.EquipmentOwned)
                    .FirstOrDefault(n => n.Id == id);
            }
        }

        public Ninja GetNinjaWithEquipmentAndClan(int id)
        {
            using (var context = new NinjaContext())
            {
                return context.Ninjas.AsNoTracking().Include(n => n.EquipmentOwned)
                    .Include(n=>n.Clan)
                    .FirstOrDefault(n => n.Id == id);
            }
        }

        public IEnumerable GetClanList()
        {
            using (var context = new NinjaContext())
            {
                return context.Clans.AsNoTracking().OrderBy(c => c.ClanName)
                    .Select(c => new { c.Id, c.ClanName }).ToList();
            }
        }

        public Ninja GetNinjaById(int id)
        {
            using (var context = new NinjaContext())
            {
                return context.Ninjas.Find(id);
                //return context.Ninjas.AsNoTracking().SingleOrDefault(n => n.Id == id);
                // Easy Coding vs. Performance
            }
        }

        public void SaveUpdatedNinja(Ninja ninja)
        {
            using (var context = new NinjaContext())
            {
                context.Entry(ninja).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void SaveNewNinja(Ninja ninja)
        {
            using (var context = new NinjaContext())
            {
                context.Ninjas.Add(ninja);
                context.SaveChanges();
            }
        }

        public void DeleteNinja(int ninjaId)
        {
            using (var context = new NinjaContext())
            {
                var ninja = context.Ninjas.Find(ninjaId);
                context.Entry(ninja).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public void SaveNewEquipment(NinjaEquipment equipment, int ninjaId)
        {
            //paying the price of not having a foreign keys!
            //reason #857 why I prefer foreign keys!
            using(var context = new NinjaContext())
            {
                var ninja = context.Ninjas.Find(ninjaId);
                ninja.EquipmentOwned.Add(equipment);

                context.SaveChanges();
            }
        }

        public void SaveUpdatedEquipment(NinjaEquipment equipment, int ninjaId)
        {
            //paying the price of not having a foreign keys!
            //reason #857 why I prefer foreign keys!
            using (var context = new NinjaContext())
            {
                ////Not Work! (There is no foreign key!)
                //context.Entry(equipment).State = EntityState.Modified;
                //var ninja = context.Ninjas.Find(ninjaId);
                //equipment.Ninja = ninja;
                //context.SaveChanges();
                var equipmentWithNinjaFromDatabase = context.Equipment.Include(n => n.Ninja)
                    .FirstOrDefault(e => e.Id == equipment.Id);
                if (equipment.Ninja.Id != ninjaId)
                {
                    var ninja = context.Ninjas.Find(ninjaId);
                    equipment.Ninja = ninja;
                }
                context.Entry(equipmentWithNinjaFromDatabase).CurrentValues.SetValues(equipment);
                context.SaveChanges();
            }
        }
    }
}
