namespace ELearningPlatform.Model.Categories.Dtos.Response;

public sealed record CategoryDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public DateTime Created { get; init; }
    public DateTime Updated { get; init; }

    
    public CategoryDto() { }

    public CategoryDto(int id, string name, DateTime created, DateTime updated)
    {
        Id = id;
        Name = name;
        Created = created;
        Updated = updated;
    }
}



