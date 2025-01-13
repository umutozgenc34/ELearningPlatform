using Core.Models;
using ELearningPlatform.Model.Order.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ELearningPlatform.Model.Order.Entities;


public class Order : IAuditEntity
{
    [BsonId] 
    [BsonRepresentation(BsonType.ObjectId)] 
    public string Id { get; set; } 

    [BsonElement("userId")] 
    public string UserId { get; set; }

    [BsonElement("orderItems")] 
    public List<OrderItem> OrderItems { get; set; } = new();

    [BsonRepresentation(BsonType.Decimal128)] 
    [BsonElement("totalPrice")] 
    public decimal TotalPrice { get; set; }

    [BsonElement("status")] 
    public OrderStatus Status { get; set; }

    [BsonElement("created")] 
    public DateTime Created { get; set; }

    [BsonElement("updated")] 
    public DateTime Updated { get; set; }
}