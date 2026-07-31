using Microsoft.EntityFrameworkCore;
using Order.Data;
using Order.Model;
using Order.Repository.Interfaces;

namespace Order.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly  OrderDbContext _context;
        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public Task<List<OrderModel>> GetOrders()
        {
           return _context.Orders.ToListAsync();

        }
        public async Task<OrderModel> GetOrderById(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            return order;
        }

        public async Task<OrderModel> CreateOrder(OrderModel orders)
        {
            _context.Orders.AddAsync(orders);
             await _context.SaveChangesAsync();
            return orders;
        }
        public async Task<OrderModel> UpdateOrder(OrderModel orders)
        {
            var result = await _context.Orders.FindAsync(orders.OrderId);
            if (result == null)
            {
                return null;
            }
            result.ProductId = orders.ProductId;
            result.Quantity = orders.Quantity;
            result.prices = orders.prices;
            result.IsPaid = orders.IsPaid;
            result.OrderDate = orders.OrderDate;
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<bool> DeleteOrder(int id)
        {
            var res = await _context.Orders.FindAsync(id);
            if (res == null) return false;
            _context.Orders.Remove(res);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
