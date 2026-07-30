using Order.DTO;

namespace Order.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderResponseDTO>> GetOrders();

        Task<OrderResponseDTO?> GetOrderById(int id);

        Task<OrderResponseDTO> CreateOrder(OrderCreateDTO order);

        Task<OrderResponseDTO?> UpdateOrder(OrderUpdateDTO order);

        Task<bool> DeleteOrder(int id);
    }
}