using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.DTO;
using ProductService.Services.Interfaces;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var response = await _productService.GetProducts();

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateDTO product)
        {
            var response = await _productService.CreateProduct(product);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return StatusCode(201, response);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductById(int id)
        {
            var response = await _productService.DeleteProduct(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var response = await _productService.GetProductById(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto product)
        {
            if (id != product.ProductId) return BadRequest("ID mismatch");

            var response = await _productService.UpdateProduct(product);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}