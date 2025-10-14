using System.ComponentModel.DataAnnotations;

namespace ServiceCenter.Models
{
    public class RepairableModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Manufacturer { get; set; }
        [StringLength(500)]
        public string Features { get; set; }

        public ICollection<FaultType>? FaultTypes { get; set; }
        public ICollection<Car>? Cars { get; set; }
    }
}
