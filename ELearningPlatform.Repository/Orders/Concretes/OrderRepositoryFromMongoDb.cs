using ELearningPlatform.Model.Order.Entities;
using ELearningPlatform.Repository.Orders.Abstracts;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ELearningPlatform.Repository.Orders.Concretes;

public class OrderRepositoryFromMongoDb : IOrderRepository
{
    private readonly IMongoCollection<Order> _orderCollection;

    public OrderRepositoryFromMongoDb(IConfiguration configuration)
    {
        var connectionStrings = configuration.GetConnectionString("MongoDb");
        var client = new MongoClient(connectionStrings);
        var database = client.GetDatabase("ELearningPlatformDb");

        _orderCollection = database.GetCollection<Order>("Orders");
    }

    public async Task<List<Order>> GetAllAsync() => await _orderCollection.Find(_ => true).ToListAsync();
    public async Task<Order> GetByIdAsync(string id) => await _orderCollection.Find(order => order.Id == id).FirstOrDefaultAsync();
    public async Task CreateAsync(Order order) => await _orderCollection.InsertOneAsync(order);
    public async Task UpdateAsync(Order order) => await _orderCollection.ReplaceOneAsync(o => o.Id == order.Id, order);
    public async Task DeleteAsync(string id) => await _orderCollection.DeleteOneAsync(order => order.Id == id);
    public async Task<List<Order>> GetOrdersByUserIdAsync(string userId) => await _orderCollection.Find(order => order.UserId == userId).ToListAsync();

}