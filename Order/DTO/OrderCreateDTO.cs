namespace Order.DTO
{
    public class OrderCreateDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public bool IsPaid { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
