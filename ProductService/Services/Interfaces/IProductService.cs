using ProductModel.Model;
using ProductService.DTO;

namespace ProductService.Services.Interfaces
{
    public interface IProductService
    {


        Task<ApiResponse<List<ProductResponseDTO>>> GetProducts();
        Task<ApiResponse<ProductResponseDTO>> GetProductById(int id);
        Task<ApiResponse<ProductResponseDTO>> CreateProduct(ProductCreateDTO product);
        Task<ApiResponse<ProductResponseDTO>> UpdateProduct(ProductUpdateDto product);
        Task<ApiResponse<bool>> DeleteProduct(int id);

    }


}

