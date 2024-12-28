using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MEV_07.Models
{
    public enum EquipmentType { Loader = 1, Digger, Mover, Lifter }
    public class Equipment
    {
        public int EquipmentId { get; set; }
        [Required, StringLength(40), Display(Name = "Eq. Code")]
        public string IdentificationCode { get; set; }
        [Required, EnumDataType(typeof(EquipmentType)), Display(Name = "Type")]
        public EquipmentType EquipmentType { get; set; }
        [Required, StringLength(40)]
        public string Model { get; set; }
        [Required, Display(Name = "Capacity (Ton)")]
        public int CapacityInTon { get; set; }
        [Required, Column(TypeName = "date"), Display(Name = "Com. Date")]
        public DateTime CommissionDate { get; set; }
        [Required, StringLength(50)]
        public string Picture { get; set; }
        public bool IsActive { get; set; }
        //Navigation
        public virtual ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
    }
    public class Maintenance
    {
        public int MaintenanceId { get; set; }
        [Required, Column(TypeName = "date"), Display(Name = "Start")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "date"), Display(Name = "End")]
        public DateTime? EndDate { get; set; }
        [Required, StringLength(150)]
        public string Comment { get; set; }
        //Fk
        [Required, ForeignKey("Equipment")]
        public int EquipmentId { get; set; }
        //Navigation
        public virtual Equipment Equipment { get; set; }

    }
    public class EquipmentDbContext : DbContext
    {
        public EquipmentDbContext()
        {
            Configuration.LazyLoadingEnabled = false;
        }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }

    }

}
