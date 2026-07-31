using System.ComponentModel.DataAnnotations;

namespace Order.Model
{
    public class OrderModel
    {
        [Key]
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public int prices { get; set; }

        public bool IsPaid { get; set; }

        public DateTime OrderDate { get; set; }
    }
}