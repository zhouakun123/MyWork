using NinjaDomain.Classes;
using NinjaDomain.Classes.Enums;
using NinjiaDomain.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new NullDatabaseInitializer<NinjaContext>());
            //InsertNinja();
            //InsertMultipleNinjas();
            //InsertNinjaWithEquipment();
            //SimpleNinjaQueries();
            //SimpleNinjaGraphQuery();
            //QueryAndUpdateNinja();
            //QueryAndUpdateNinjaDisconnected();
            //DeleteNinja();
            //DeleteNinjaWithKeyValue();
            //DeleteNinjaViaStoredProcedure();
            //RetrieveDataWithFind();
            //RetrieveDataWithStoredProc();
            //ProjectionQuery();
            Console.ReadKey();
        }

        private static void ProjectionQuery()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninjas = context.Ninjas
                    .Select(n => new { n.Name, n.DateOfBirth, n.EquipmentOwned })
                    .ToList();
                //Return Anonymous Types
            }
        }

        private static void SimpleNinjaGraphQuery()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                // No equipment come out with the ninja
                //var ninja = context.Ninjas.
                //    FirstOrDefault(n => n.Name.StartsWith("Kacy"));

                //Eager Loading : careful! it may be too costy
                //var ninja = context.Ninjas.Include(n => n.EquipmentOwned)//.Include(n=>n.Clan).Include(n=>n.Clan.Ninjas)
                //    .FirstOrDefault(n => n.Name.StartsWith("Kacy"));
                //context.Ninjas.Include("EquipmentOwned");

                //Explicit loading
                var ninja = context.Ninjas
                    .FirstOrDefault(n => n.Name.StartsWith("Kacy"));
                Console.WriteLine("Ninja Retrieved:" + ninja.Name);
                //context.Entry(ninja).Collection(n => n.EquipmentOwned).Load();
                //Lazy Loading : make the Equipment virtual !!BUT it is very dangerous to use it!!!
                Console.WriteLine("Ninja Equipment Count: {0}", ninja.EquipmentOwned.Count);

                // Write Projections
            }
        }

        private static void InsertNinjaWithEquipment()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninja = new Ninja
                {
                    Name = "Kacy Catanzaro",
                    ServedInOniwaban = false,
                    DateOfBirth = new DateTime(1990, 1, 1),
                    ClanId = 1
                };
                var muscles = new NinjaEquipment
                {
                    Name = "Muscles",
                    Type = EquipmentType.Tool
                };
                var spunk = new NinjaEquipment
                {
                    Name = "Spunk",
                    Type = EquipmentType.Weapon
                };
                ninja.EquipmentOwned.Add(muscles);
                ninja.EquipmentOwned.Add(spunk);
                context.Ninjas.Add(ninja);              
                context.SaveChanges();
            }
        }

        private static void DeleteNinjaViaStoredProcedure()
        {
            var keyval = 3;
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                context.Database.ExecuteSqlCommand("exec DeleteNinjaViaId {0}", keyval);
            }
        }

        private static void DeleteNinjaWithKeyValue()
        {
            var keyval = 8;
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninja = context.Ninjas.Find(keyval);
                context.Ninjas.Remove(ninja);
                context.SaveChanges();
            }
        }

        private static void DeleteNinja()
        {
            Ninja ninja;
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                ninja = context.Ninjas.FirstOrDefault();
                //context.Ninjas.Remove(ninja);
                //context.SaveChanges();
            }
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                //context.Ninjas.Attach(ninja);
                //context.Ninjas.Remove(ninja);
                context.Entry(ninja).State = EntityState.Deleted;// EF will attatch it for you!
                context.SaveChanges();
            }
        }

        private static void RetrieveDataWithStoredProc()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninjas = context.Ninjas.SqlQuery("exec GetOldNinjas").ToList();
                foreach(var ninja in ninjas)
                {
                    Console.WriteLine(ninja.Name);
                }
            }
        }

        private static void RetrieveDataWithFind()
        {
            var keyval = 6;
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                // Will first check it is already in memory, if not, it will query from 
                // the database.
                var ninja = context.Ninjas.Find(keyval);// Only one can be found, otherwise it will be failed.
                Console.WriteLine("After Find#1" + ninja.Name);

                // If it is already in memory, 'Find' will not execute query again.
                var someNinja = context.Ninjas.Find(keyval);
                Console.WriteLine("After Find#2" + someNinja.Name);
                ninja = null;
            }
        }

        private static void InsertNinja()
        {
            var ninja1 = new Ninja
            {
                Name = "Leonardo",
                ServedInOniwaban = false,
                DateOfBirth = new DateTime(1984, 1, 1),
                ClanId = 1
            };

            var ninja2 = new Ninja
            {
                Name = "Raphael",
                ServedInOniwaban = false,
                DateOfBirth = new DateTime(1985, 1, 1),
                ClanId = 1
            };
            //EntityFramework.BulkInsert
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                context.Ninjas.AddRange(new List<Ninja> { ninja1, ninja2 });
                context.SaveChanges();
            }
        }

        private static void SimpleNinjaQueries()
        {
            using (var context = new NinjaContext())
            {
                var ninjas = context.Ninjas.ToList();
                //var query = context.Ninjas;
                //var someninjas = query.ToList();//execute query
                //foreach (var ninja in query)
                //foreach (var ninja in context.Ninjas) // DB connection will keep openning until the loop end
                //                                      //!!!Avoid doing lots of work in an enumeration that is also responsible for query execution
                //{
                //    Console.WriteLine(ninja.Name);
                //}
                var ninjia = context.Ninjas.
                    Where(n => n.DateOfBirth >= new DateTime(1984, 1, 1))
                    .FirstOrDefault();

            }
        }

        private static void QueryAndUpdateNinja()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninja = context.Ninjas.FirstOrDefault();
                ninja.ServedInOniwaban = (!ninja.ServedInOniwaban);
                context.SaveChanges();
            }
        }

        private static void QueryAndUpdateNinjaDisconnected()
        {
            Ninja ninja;
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                ninja = context.Ninjas.FirstOrDefault();
            }

            ninja.ServedInOniwaban = (!ninja.ServedInOniwaban);

            // The changes will never get excuted, if just use the SaveChanges.
            // Learn more in Entity Framework in the Enterprise
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                //context.Ninjas.Attach(ninja);
                context.Entry(ninja).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }

}
