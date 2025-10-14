using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ServiceCenter.Models
{
    public class FaultType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        [StringLength(500)]
        public string RepairMethods { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal RepairCost { get; set; }

        [ForeignKey("Model")]
        public int ModelId { get; set; }
        public RepairableModel Model { get; set; }
        public ICollection<OrderFault> OrderFaults { get; set; }
    }
}
