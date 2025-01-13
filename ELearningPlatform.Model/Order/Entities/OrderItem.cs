using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class OrderItem
{
    [BsonElement("courseId")]
    [BsonRepresentation(BsonType.String)]
    public Guid CourseId { get; set; }

    [BsonElement("courseName")]
    public string CourseName { get; set; }

    [BsonRepresentation(BsonType.Decimal128)] 
    [BsonElement("price")] 
    public decimal Price { get; set; }

    [BsonIgnore] 
    public decimal TotalPrice => Price;
}