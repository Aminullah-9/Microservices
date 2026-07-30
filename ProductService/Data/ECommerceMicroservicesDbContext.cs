using Microsoft.EntityFrameworkCore;
using ProductModel.Model;
using System.Collections.Generic;

namespace ProductService.Data
{
    

    
        public class EcommerceApiDbContext:DbContext
        {
            public EcommerceApiDbContext(DbContextOptions<EcommerceApiDbContext> options) : base(options)
            {
            }
           
            public DbSet<Product> Products { get; set; }
 
        }
   

}
