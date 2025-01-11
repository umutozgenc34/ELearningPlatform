using ELearningPlatform.Model.Categories.Dtos.Response;

namespace ELearningPlatform.Model.Courses.Dtos.Response;

public sealed record CourseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public string ImageUrl { get; init; }
    public DateTime Created { get; init; }
    public DateTime Updated { get; init; }
    public CategoryDto Category { get; init; }
    public CourseDetailsDto CourseDetails { get; init; }

    public CourseDto() { }

    public CourseDto(Guid id, string name, string description, decimal price, string imageUrl, DateTime created, DateTime updated, CategoryDto category, CourseDetailsDto courseDetails)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        Created = created;
        Updated = updated;
        Category = category;
        CourseDetails = courseDetails;
    }        
    
}