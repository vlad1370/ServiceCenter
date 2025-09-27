using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ServiceCenter.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "Серийный номер должен содержать 17 символов")]
        public string SerialNumber { get; set; }

        [ForeignKey("Model")]
        public int ModelId { get; set; }
        public RepairableModel? Model { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
