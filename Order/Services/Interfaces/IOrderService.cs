using Order.DTO;

public interface IOrderService
{
    Task<ApiResponse<List<OrderResponseDTO>>> GetOrders();

    Task<ApiResponse<OrderResponseDTO>> GetOrderById(int id);

    Task<ApiResponse<OrderResponseDTO>> CreateOrder(OrderCreateDTO order);

    Task<ApiResponse<OrderResponseDTO>> UpdateOrder(OrderUpdateDTO order);

    Task<ApiResponse<bool>> DeleteOrder(int id);
}