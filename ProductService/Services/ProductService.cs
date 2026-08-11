using Microsoft.AspNetCore.Http;
using ProductModel.Model;
using ProductService.DTO;
using ProductService.Repository;
using ProductService.Repository.Interfaces;
using ProductService.Services.Interfaces;

namespace ProductService.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<List<ProductResponseDTO>>> GetProducts()
        {
            var products = await _repository.GetProducts();

            var data = products.Select(pro => new ProductResponseDTO
            {
                ProductId = pro.ProductId,
                ProductName = pro.ProductName,
                ProductDescription = pro.ProductDescription,
                Price = pro.ProductPrice,
                ProductQuantity = pro.ProductQuantity
            }).ToList();

            if (data.Count == 0)
            {
                return new ApiResponse<List<ProductResponseDTO>>
                {
                    Success = false,
                    Message = "No Products Available.",
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = data
                };
            }

            return new ApiResponse<List<ProductResponseDTO>>
            {
                Success = true,
                Message = "Products fetched successfully.",
                StatusCode = StatusCodes.Status200OK,
                Data = data
            };
        }

        public async Task<ApiResponse<ProductResponseDTO>> GetProductById(int id)
        {
            var product = await _repository.GetProductById(id);

            if (product == null)
            {
                return new ApiResponse<ProductResponseDTO>
                {
                    Success = false,
                    Message = "Product not found.",
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null
                };
            }

            var data = new ProductResponseDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                Price = product.ProductPrice,
                ProductQuantity = product.ProductQuantity
            };

            return new ApiResponse<ProductResponseDTO>
            {
                Success = true,
                Message = "Product fetched successfully.",
                StatusCode = StatusCodes.Status200OK,
                Data = data
            };
        }

        public async Task<ApiResponse<ProductResponseDTO>> CreateProduct(ProductCreateDTO product)
        {
            var newProduct = new Product
            {
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductPrice = product.Price,
                ProductQuantity = product.ProductQuantity
            };

            await _repository.CreateProduct(newProduct);

            var data = new ProductResponseDTO
            {
                ProductId = newProduct.ProductId,
                ProductName = newProduct.ProductName,
                ProductDescription = newProduct.ProductDescription,
                Price = newProduct.ProductPrice,
                ProductQuantity = newProduct.ProductQuantity
            };

            return new ApiResponse<ProductResponseDTO>
            {
                Success = true,
                Message = "Product created successfully.",
                StatusCode = StatusCodes.Status201Created,
                Data = data
            };
        }

        public async Task<ApiResponse<ProductResponseDTO>> UpdateProduct(ProductUpdateDto productUpdateDto)
        {
            var product = new Product
            {
                ProductId = productUpdateDto.ProductId,
                ProductName = productUpdateDto.ProductName,
                ProductDescription = productUpdateDto.ProductDescription,
                ProductPrice = productUpdateDto.Price,
                ProductQuantity = productUpdateDto.ProductQuantity
            };

            var updatedProduct = await _repository.UpdateProduct(product);

            if (updatedProduct == null)
            {
                return new ApiResponse<ProductResponseDTO>
                {
                    Success = false,
                    Message = "Product not found.",
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null
                };
            }

            var data = new ProductResponseDTO
            {
                ProductId = updatedProduct.ProductId,
                ProductName = updatedProduct.ProductName,
                ProductDescription = updatedProduct.ProductDescription,
                Price = updatedProduct.ProductPrice,
                ProductQuantity = updatedProduct.ProductQuantity
            };

            return new ApiResponse<ProductResponseDTO>
            {
                Success = true,
                Message = "Product updated successfully.",
                StatusCode = StatusCodes.Status200OK,
                Data = data
            };
        }

        public async Task<ApiResponse<bool>> DeleteProduct(int id)
        {
            var product = await _repository.GetProductById(id);

            if (product == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Product not found.",
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = false
                };
            }

            var result = await _repository.DeleteProduct(id);

            return new ApiResponse<bool>
            {
                Success = result,
                Message = result
                    ? "Product deleted successfully."
                    : "Failed to delete product.",
                StatusCode = result ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest,
                Data = result
            };
        }
    }
}