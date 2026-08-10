namespace Order.DTO
{
    public class OrderResponseDTO
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int prices { get; set; }
        public bool IsPaid { get; set; }
        public DateTime OrderDate { get; set; }

        public string UserId { get; set; } 
    }
}
