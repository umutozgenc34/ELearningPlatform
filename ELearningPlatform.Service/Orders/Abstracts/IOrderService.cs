using Core.Response;
using ELearningPlatform.Model.Order.Entities;
using ELearningPlatform.Model.Order.Enums;

namespace ELearningPlatform.Service.Orders.Abstracts;

public interface IOrderService
{
    Task<ServiceResult> CreateOrderAsync(string coupon);
    Task<ServiceResult<Order>> GetOrderByIdAsync(string orderId);
    Task<ServiceResult<List<Order>>> GetUserOrdersAsync();
    Task<ServiceResult> DeleteOrderAsync(string orderId);
    Task<ServiceResult<List<Order>>> GetAllOrdersAsync();
    Task<ServiceResult> UpdateOrderStatusAsync(string orderId, OrderStatus status);

}
