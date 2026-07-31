using Order.DTO;
using Order.Model;
using Order.Repository;
using Order.Repository.Interfaces;
using Order.Services.Interfaces;
using System.Reflection.Metadata.Ecma335;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Order.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient, IOrderRepository orderRepository)
        {
            _httpClient = httpClient;
            _orderRepository = orderRepository;
        }


        public async Task<ApiResponse<List<OrderResponseDTO>>> GetOrders()
        {
            var orders = await _orderRepository.GetOrders();

            var data = orders.Select(x => new OrderResponseDTO
            {
                OrderId = x.OrderId,
                ProductId = x.ProductId,
                OrderDate = x.OrderDate,
                IsPaid = x.IsPaid,
                prices = x.prices,
                Quantity = x.Quantity
            }).ToList();

            return new ApiResponse<List<OrderResponseDTO>>
            {
                Success = true,
                Message = "Orders fetched successfully.",
                Data = data
            };
        }

        public async Task<ApiResponse<OrderResponseDTO>> GetOrderById(int id)
        {
            var order = await _orderRepository.GetOrderById(id);

            if (order == null)
            {
                return new ApiResponse<OrderResponseDTO>
                {
                    Success = false,
                    Message = "Order not found.",
                    Data = null
                };
            }

            var data = new OrderResponseDTO
            {
                OrderId = order.OrderId,
                ProductId = order.ProductId,
                OrderDate = order.OrderDate,
                IsPaid = order.IsPaid,
                prices = order.prices,
                Quantity = order.Quantity
            };

            return new ApiResponse<OrderResponseDTO>
            {
                Success = true,
                Message = "Order fetched successfully.",
                Data = data
            };
        }

        public async Task<ApiResponse<OrderResponseDTO>> CreateOrder(OrderCreateDTO orderCreateDTO)
        {
            if (orderCreateDTO.ProductId <= 0)
            {
                return new ApiResponse<OrderResponseDTO>
                {
                    Success = false,
                    Message = "Invalid Product Id.",
                    Data = null
                };
            }

            if (orderCreateDTO.Quantity <= 0)
            {
                return new ApiResponse<OrderResponseDTO>
                {
                    Success = false,
                    Message = "Quantity must be greater than zero.",
                    Data = null
                };
            }


            var product = await GetProductFromProductService(orderCreateDTO.ProductId);

            if (product == null)
            {
                return new ApiResponse<OrderResponseDTO>
                {
                    Success = false,
                    Message = "Product does not exist.",
                    Data = null
                };
            }


            if (orderCreateDTO.Quantity > product.ProductQuantity)
            {
                return new ApiResponse<OrderResponseDTO>
                {
                    Success = false,
                    Message = "Requested quantity is greater than available stock.",
                    Data = null
                };
            }


            var order = new OrderModel
            {
                ProductId = orderCreateDTO.ProductId,
                OrderDate = orderCreateDTO.OrderDate,
                IsPaid = orderCreateDTO.IsPaid,
                prices = orderCreateDTO.prices,
                Quantity = orderCreateDTO.Quantity
            };


            await _orderRepository.CreateOrder(order);


            var data = new OrderResponseDTO
            {
                OrderId = order.OrderId,
                ProductId = order.ProductId,
                OrderDate = order.OrderDate,
                IsPaid = order.IsPaid,
                prices = order.prices,
                Quantity = order.Quantity
            };


            return new ApiResponse<OrderResponseDTO>
            {
                Success = true,
                Message = "Order created successfully.",
                Data = data
            };
        }
        public async Task<ApiResponse<OrderResponseDTO>> UpdateOrder(OrderUpdateDTO orderUpdateDTO)
        {
            var existingOrder = await _orderRepository.GetOrderById(orderUpdateDTO.OrderId);

            if (existingOrder == null)
            {
                return new ApiResponse<OrderResponseDTO>
                {
                    Success = false,
                    Message = "Order not found.",
                    Data = null
                };
            }


            var order = new OrderModel
            {
                OrderId = orderUpdateDTO.OrderId,
                ProductId = orderUpdateDTO.ProductId,
                OrderDate = orderUpdateDTO.OrderDate,
                IsPaid = orderUpdateDTO.IsPaid,
                prices = orderUpdateDTO.prices,
                Quantity = orderUpdateDTO.Quantity
            };


            await _orderRepository.UpdateOrder(order);


            var data = new OrderResponseDTO
            {
                OrderId = order.OrderId,
                ProductId = order.ProductId,
                OrderDate = order.OrderDate,
                IsPaid = order.IsPaid,
                prices = order.prices,
                Quantity = order.Quantity
            };


            return new ApiResponse<OrderResponseDTO>
            {
                Success = true,
                Message = "Order updated successfully.",
                Data = data
            };
        }
        public async Task<ApiResponse<bool>> DeleteOrder(int id)
        {
            var order = await _orderRepository.GetOrderById(id);

            if (order == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Order not found.",
                    Data = false
                };
            }


            var result = await _orderRepository.DeleteOrder(id);


            return new ApiResponse<bool>
            {
                Success = result,
                Message = result
                    ? "Order deleted successfully."
                    : "Failed to delete order.",
                Data = result
            };
        }
        private async Task<ProductResponseDto?> GetProductFromProductService(int productId)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7257/api/Product/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var product = await response.Content.ReadFromJsonAsync<ProductResponseDto>();

            return product;
        }
    }
}
