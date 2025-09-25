using System.ComponentModel.DataAnnotations;

namespace ServiceCenter.Models
{
    public class RepairableModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Features { get; set; }

        public ICollection<FaultType>? FaultTypes { get; set; }
        public ICollection<Car>? Cars { get; set; }
    }
}
