using ProductModel.Model;

namespace ProductService.Repository.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProducts();
        Task<Product?> GetProductById(int id);
        Task<Product> CreateProduct(Product product);
        Task<Product?> UpdateProduct(Product product);
        Task<bool> DeleteProduct(int id);
    }
}

