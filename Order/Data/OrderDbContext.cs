using Microsoft.EntityFrameworkCore;
using Order.Model;

namespace Order.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(
            DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }

        public DbSet<OrderModel> Orders { get; set; }
    }
}