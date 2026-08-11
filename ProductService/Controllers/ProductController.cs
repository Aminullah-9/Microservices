using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductModel.Model;
using ProductService.DTO;
using ProductService.Services.Interfaces;
using System.Reflection.Metadata.Ecma335;

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
        public async Task<ActionResult> GetProducts()
        {
            var products = await _productService.GetProducts();
            if (products == null) 
            { 
                return NotFound("No Products Available");
            }
            return Ok(products);
        }
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(ProductCreateDTO product)
        {
            await _productService.CreateProduct(product);

            return Ok(product);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductById(int id)
        {
            var item = await _productService.GetProductById(id);
            if (item == null)
            {
                return BadRequest();
            }
            await _productService.DeleteProduct(id);
            return Ok("Item Deleted Succesfully");
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductById(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto product)
        {
            if (id != product.ProductId) return BadRequest("ID mismatch");
            var updated = await _productService.UpdateProduct(product);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
    }
}
