using Core.Models;
using ELearningPlatform.Model.Courses.Entities;

namespace ELearningPlatform.Model.Lessons.Entity;

public sealed class Lesson : BaseEntity<int>  , IAuditEntity
{

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = default!;
    public string? Title { get; set; }
    
    public string VideoUrl { get; set; } = default!;

    public int LessonOrder { get; set; }
    public DateTime Created { get ; set ; }
    public DateTime Updated { get; set; }
}
