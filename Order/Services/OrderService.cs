using Order.DTO;
using Order.Model;
using Order.Repository;
using Order.Services.Interfaces;

namespace Order.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderRepository _orderService;

        public OrderService(OrderRepository orderService)
        {
            _orderService = orderService;
        }

        public async Task<List<OrderResponseDTO>> GetOrders()
        {
            var orders = await _orderService.GetOrder();

            return orders.Select(x => new OrderResponseDTO
            {
                OrderId = x.OrderId,
                ProductId = x.ProductId,
                OrderDate = x.OrderDate,
                IsPaid = x.IsPaid,
                prices = x.prices,
                Quantity = x.Quantity
            }).ToList();
        }

        public async Task<OrderResponseDTO?> GetOrderbyId(int id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
            {
                return null;
            }
            return new OrderResponseDTO
            {
                OrderId = order.OrderId,
                ProductId = order.ProductId,
                OrderDate = order.OrderDate,
                IsPaid = order.IsPaid,
                prices = order.prices,
                Quantity = order.Quantity
            };
        }

        public async Task<OrderResponseDTO> CreateOrder(OrderCreateDTO orderCreateDTO)
        {
            var Order = new OrderModel
            {
                OrderId = orderCreateDTO.OrderId,
                ProductId = orderCreateDTO.ProductId,
                OrderDate = orderCreateDTO.OrderDate,
                IsPaid = orderCreateDTO.IsPaid,
                prices = orderCreateDTO.prices,
                Quantity = orderCreateDTO.Quantity
            };
            await _orderService.CreateOrder(Order);

            return new OrderResponseDTO
            {
                OrderId = Order.OrderId,
                ProductId = Order.ProductId,
                OrderDate = Order.OrderDate,
                IsPaid = Order.IsPaid,
                prices = Order.prices,
                Quantity = Order.Quantity
            };
        }

        public async Task<OrderResponseDTO> UpdateOrder(OrderUpdateDTO orderUpdateDTO)
        {
            var Order = new OrderModel
            {
                OrderId = orderUpdateDTO.OrderId,
                ProductId = orderUpdateDTO.ProductId,
                OrderDate = orderUpdateDTO.OrderDate,
                IsPaid = orderUpdateDTO.IsPaid,
                prices = orderUpdateDTO.prices,
                Quantity = orderUpdateDTO.Quantity
            };
            await _orderService.UpdateOrder(Order);

            return new OrderResponseDTO
            {
                OrderId = Order.OrderId,
                ProductId = Order.ProductId,
                OrderDate = Order.OrderDate,
                IsPaid = Order.IsPaid,
                prices = Order.prices,
                Quantity = Order.Quantity
            };
        }
        public async Task<bool> DeleteOrder(int id)
        {
            return await _orderService.DeleteOrder(id);
        }
    }
}
