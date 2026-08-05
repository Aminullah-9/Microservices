using Order.DTO;
using Order.Model;
using Order.Repository;
using Order.Repository.Interfaces;
using System.Reflection.Metadata.Ecma335;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Order.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly HttpClient _httpClient;

        public OrderService(IHttpClientFactory httpClientFactory, IOrderRepository orderRepository)
        {
            _httpClient = httpClientFactory.CreateClient("ProductService");
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
                StatusCode = StatusCodes.Status200OK,
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
                    StatusCode = StatusCodes.Status404NotFound,
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
                StatusCode = StatusCodes.Status200OK,
                Data = data
            };
        }

        public async Task<ApiResponse<OrderResponseDTO>> CreateOrder(OrderCreateDTO orderCreateDTO)
        {
            
            var product = await GetProductFromProductService(orderCreateDTO.ProductId);

            if (product == null)
            {
                return new ApiResponse<OrderResponseDTO>
                {
                    Success = false,
                    Message = "Product does not exist.",
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null
                };
            }


            if (orderCreateDTO.Quantity > product.ProductQuantity)
            {
                return new ApiResponse<OrderResponseDTO>
                {
                    Success = false,
                    Message = "Requested quantity is greater than available stock.",
                    StatusCode = StatusCodes.Status400BadRequest,
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
                StatusCode = StatusCodes.Status200OK,
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
                    StatusCode = StatusCodes.Status404NotFound,
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
                StatusCode = StatusCodes.Status200OK,
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
                    StatusCode = StatusCodes.Status404NotFound,
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
                StatusCode=result?StatusCodes.Status410Gone:StatusCodes.Status400BadRequest,
                Data = result
            };
        }
        private async Task<ProductResponseDto?> GetProductFromProductService(int productId)
        {
            var response = await _httpClient.GetAsync($"/api/Product/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var product = await response.Content.ReadFromJsonAsync<ProductResponseDto>();

            return product;
        }
    }
}
