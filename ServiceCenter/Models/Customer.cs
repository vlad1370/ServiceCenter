using System.ComponentModel.DataAnnotations;

namespace ServiceCenter.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Range(18, 100)]
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }

        [Required]
        public string Phone { get; set; }
        public ICollection<Car>? Cars { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
