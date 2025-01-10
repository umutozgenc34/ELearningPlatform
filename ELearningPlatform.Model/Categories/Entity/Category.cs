using Core.Models;

namespace ELearningPlatform.Model.Categories.Entity;

public sealed class Category : BaseEntity<int> , IAuditEntity
{
    public string Name { get; set; } = default!;
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
