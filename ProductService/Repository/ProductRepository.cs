using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductModel.Model;
using ProductService.Data;
using ProductService.Repository.Interfaces;

namespace ProductService.Repository
{
    public class ProductRepository:IProductRepository
    {
        public readonly EcommerceApiDbContext _context;

        public ProductRepository(EcommerceApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> CreateProduct(Product product)
        {
            _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> GetProductById(int Id)
        {
            var product = await _context.Products.FindAsync(Id);
            return product;
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            var result = await _context.Products.FindAsync(product.ProductId);
            if (result == null)
            {
                return null;
            }
            result.ProductName = product.ProductName;
            result.ProductDescription = product.ProductDescription;
            result.ProductPrice = product.ProductPrice;
            result.ProductQuantity = product.ProductQuantity;
             await _context.SaveChangesAsync();
            return result;

        }
        public async Task<bool> DeleteProduct(int id)
        {
            var res= await _context.Products.FindAsync(id);
            if(res == null) return false;
            _context.Products.Remove(res);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
