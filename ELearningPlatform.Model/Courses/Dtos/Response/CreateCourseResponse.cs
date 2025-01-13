namespace ELearningPlatform.Model.Courses.Dtos.Response;

public record CreateCourseResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime Created { get; set; }
    public string CategoryName { get; set; }
    public CourseDetailsDto CourseDetails { get; set; }


    public CreateCourseResponse() { }

    public CreateCourseResponse(Guid id, string name, string description, decimal price, string? imageUrl, DateTime created, string categoryName, CourseDetailsDto courseDetails)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        Created = created;
        CategoryName = categoryName;
        CourseDetails = courseDetails;
    }
}
