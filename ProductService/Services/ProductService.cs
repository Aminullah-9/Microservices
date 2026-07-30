using ProductModel.Model;
using ProductService.DTO;
using ProductService.Repository;
using ProductService.Repository.Interfaces;
using ProductService.Services.Interfaces;

namespace ProductService.Services
{
    public class ProductService: IProductService
    {
        private readonly IProductRepository _repository;
        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductResponseDTO>> GetProducts()
        {
            var pro= await _repository.GetProducts();
            return pro.Select(pro=>new ProductResponseDTO
            {
                ProductId = pro.ProductId,
                ProductName = pro.ProductName,
                ProductDescription = pro.ProductDescription,
                Price=pro.ProductPrice,
                ProductQuantity = pro.ProductQuantity
            }).ToList();
        } 

        public async Task<ProductResponseDTO> GetProductById(int id)
        {
            var product = await _repository.GetProductById(id);
            return new ProductResponseDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                Price = product.ProductPrice,
                ProductQuantity = product.ProductQuantity
            };
        } 

        public async Task<ProductResponseDTO> CreateProduct(ProductCreateDTO product)
        {
            var Product = new Product
            {
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductPrice = product.Price,
                ProductQuantity = product.ProductQuantity
            };
                
            await _repository.CreateProduct(Product);
            return new ProductResponseDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                Price = product.Price,
                ProductQuantity = product.ProductQuantity
            };
        }

        public async Task<ProductResponseDTO?> UpdateProduct(
         ProductUpdateDto productUpdateDto)
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
                return null;
            }

            return new ProductResponseDTO
            {
                ProductId = updatedProduct.ProductId,
                ProductName = updatedProduct.ProductName,
                ProductDescription = updatedProduct.ProductDescription,
                Price = updatedProduct.ProductPrice,
                ProductQuantity = updatedProduct.ProductQuantity
            };
        }

        public async Task<bool> DeleteProduct(int id)
        {
            return await _repository.DeleteProduct(id);
        }
    }
}
