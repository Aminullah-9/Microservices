using Order.Model;

namespace Order.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<OrderModel>> GetOrders();
        Task<OrderModel> GetOrderById(int id);
        Task<OrderModel?> CreateOrder(OrderModel order);
        Task<OrderModel?> UpdateOrder(OrderModel order);
        Task<bool> DeleteOrder(int id);
    }
}
