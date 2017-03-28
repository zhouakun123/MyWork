using NinjaDomain.Classes.Enums;
using NinjaDomain.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaDomain.Classes
{
    public class Ninja : IMotificationHistory
    {
        public Ninja()
        {
            EquipmentOwned = new List<NinjaEquipment>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool ServedInOniwaban { get; set; }
        public Clan Clan { get; set; }
        public int ClanId { get; set; }
        public /*virtual*/ List<NinjaEquipment> EquipmentOwned { get; set; }
        public DateTime DateOfBirth { get; set; }

        public DateTime DateModified {get;set; }

        public DateTime DateCreated
        {
            get;

            set;
        }

        public bool IsDirty
        {
            get;

            set;
        }
    }

    public class Clan : IMotificationHistory
    {
        public Clan()
        {
            Ninjas = new List<Ninja>();
        }
        public int Id { get; set; }
        public string ClanName { get; set; }
        public List<Ninja> Ninjas { get; set; }

        public DateTime DateModified
        {
            get;

            set;
        }

        public DateTime DateCreated
        {
            get;

            set;
        }

        public bool IsDirty
        {
            get;

            set;
        }
    }
    public class NinjaEquipment : IMotificationHistory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EquipmentType Type { get; set; }
        [Required]
        public Ninja Ninja { get; set; }

        public DateTime DateModified
        {
            get; set;
        }

        public DateTime DateCreated
        {
            get;

            set;
        }

        public bool IsDirty
        {
            get;

            set;
        }
    }
}
