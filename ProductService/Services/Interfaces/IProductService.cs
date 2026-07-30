using ProductModel.Model;
using ProductService.DTO;

namespace ProductService.Services.Interfaces
{
    public interface IProductService
    {
        
        
            Task<List<ProductResponseDTO>> GetProducts();
            Task<ProductResponseDTO?> GetProductById(int id);
            Task<ProductResponseDTO> CreateProduct(ProductCreateDTO product);
            Task<ProductResponseDTO?> UpdateProduct(ProductUpdateDto product);
            Task<bool> DeleteProduct(int id);
        
    }


}

