namespace ProductService.DTO
{
    public class ProductCreateDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int Price { get; set; }
        public int ProductQuantity { get; set; }

    }
}
