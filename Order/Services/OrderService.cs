using Azure.Core;
using Order.DTO;
using Order.Model;
using Order.Repository;
using Order.Repository.Interfaces;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Order.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderService(IHttpClientFactory httpClientFactory, IOrderRepository orderRepository, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("ProductService");
            _orderRepository = orderRepository;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<ApiResponse<List<OrderResponseDTO>>> GetOrders()
        {
            var user= _httpContextAccessor.HttpContext.User;

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return new ApiResponse<List<OrderResponseDTO>>
                {
                    Success = false,
                    Message = "User Identity not Found.",
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Data = null
                };
            }
            
            var IsAdmin= user.IsInRole("Admin");
            var orders = await _orderRepository.GetOrders();

            if(!IsAdmin)
            {
                orders = orders.Where(o => o.UserId == userId).ToList();
            }

            var data = orders.Select(x => new OrderResponseDTO
            {
                OrderId = x.OrderId,
                ProductId = x.ProductId,
                OrderDate = x.OrderDate,
                IsPaid = x.IsPaid,
                prices = x.prices,
                Quantity = x.Quantity,
                UserId = x.UserId
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
                Quantity = order.Quantity,
                UserId = order.UserId
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
            Console.WriteLine($"Product ID: {product?.ProductId}");
            Console.WriteLine($"Product Quantity: {product?.ProductQuantity}");
            Console.WriteLine($"Requested Quantity: {orderCreateDTO.Quantity}");
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

           

            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return new ApiResponse<OrderResponseDTO>
                {
                    Success = false,
                    Message = "User identity not found.",
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Data = null
                };
            }

            var order = new OrderModel
            {
                ProductId = orderCreateDTO.ProductId,
                OrderDate = orderCreateDTO.OrderDate,
                IsPaid = orderCreateDTO.IsPaid,
                Quantity = orderCreateDTO.Quantity,
                UserId = userId ?? string.Empty
            };


            await _orderRepository.CreateOrder(order);

            var StockReduced = await ReduceStoke(orderCreateDTO.ProductId, orderCreateDTO.Quantity);

            if (!StockReduced)
            {
                return new ApiResponse<OrderResponseDTO>
                {
                    Success = false,
                    Message = "Unable to update product stock.",
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
            }

            var data = new OrderResponseDTO
            {
                OrderId = order.OrderId,
                ProductId = order.ProductId,
                OrderDate = order.OrderDate,
                IsPaid = order.IsPaid,
                prices = product.Price * orderCreateDTO.Quantity,
                Quantity = order.Quantity,
                UserId= order.UserId
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
                Quantity = order.Quantity,
                UserId = existingOrder.UserId
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
            var token = _httpContextAccessor.HttpContext?
           .Request.Headers["Authorization"]
            .FirstOrDefault();

            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/Product/{productId}");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.TryAddWithoutValidation(
                    "Authorization",
                    token);
            }

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result =
                await response.Content.ReadFromJsonAsync<
                    ApiResponse<ProductResponseDto>>();

            return result?.Data;
        }

        private async Task<bool> ReduceStoke(int productId, int quantity)
        {
            var token = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"]
                .FirstOrDefault();

            using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/Product/{productId}/reduce-stock");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.TryAddWithoutValidation(
                    "Authorization",
                    token);
            }

            request.Content = JsonContent.Create(new
            {
                quantity = quantity
            });

            var response = await _httpClient.SendAsync(request);

            Console.WriteLine($"Reduce Stock Status: {response.StatusCode}");

            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Reduce Stock Response: {responseBody}");

            return response.IsSuccessStatusCode;
        }

    }
}
