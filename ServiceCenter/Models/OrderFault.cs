using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ServiceCenter.Models
{
    public class OrderFault
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [ForeignKey("FaultType")]
        public int FaultTypeId { get; set; }
        public FaultType FaultType { get; set; }
    }
}
