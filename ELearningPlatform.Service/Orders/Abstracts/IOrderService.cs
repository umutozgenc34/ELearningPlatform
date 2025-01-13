using Core.Response;
using ELearningPlatform.Model.Order.Entities;

namespace ELearningPlatform.Service.Orders.Abstracts;

public interface IOrderService
{
    Task<ServiceResult> CreateOrderAsync();
    Task<ServiceResult<Order>> GetOrderByIdAsync(string orderId);
    Task<ServiceResult<List<Order>>> GetUserOrdersAsync();
    Task<ServiceResult> DeleteOrderAsync(string orderId);
    Task<ServiceResult<List<Order>>> GetAllOrdersAsync();

}
