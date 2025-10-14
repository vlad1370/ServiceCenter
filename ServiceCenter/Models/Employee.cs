using System.ComponentModel.DataAnnotations;

namespace ServiceCenter.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Range(18, 100)]
        public int Age { get; set; }
        [StringLength(10)]
        public string Gender { get; set; }
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        [StringLength(50)]
        public string Position { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
