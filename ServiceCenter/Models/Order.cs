using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ServiceCenter.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public bool HasWarranty { get; set; }

        public int? WarrantyPeriodDays { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        [StringLength(17)]
        public string CarSerialNumber { get; set; }

        [ForeignKey("Car")]
        public int CarId { get; set; }
        public Car? Car { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public ICollection<OrderFault>? OrderFaults { get; set; }
    }
}
