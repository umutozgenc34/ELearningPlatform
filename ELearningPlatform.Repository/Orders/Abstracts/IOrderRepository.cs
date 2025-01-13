using ELearningPlatform.Model.Order.Entities;

namespace ELearningPlatform.Repository.Orders.Abstracts;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync();
    Task<Order> GetByIdAsync(string id);
    Task CreateAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(string id);
    Task<List<Order>> GetOrdersByUserIdAsync(string userId);
}
