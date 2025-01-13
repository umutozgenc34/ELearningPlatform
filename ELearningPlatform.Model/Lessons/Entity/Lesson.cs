using Core.Models;

namespace ELearningPlatform.Model.Lessons.Entity
{
    public sealed class Lesson : BaseEntity<int>  , IAuditEntity
    {

        public Guid CourseId { get; set; }
        public string? Title { get; set; }
        
        public string VideoUrl { get; set; } = default!;

        public int LessonOrder { get; set; }
        public DateTime Created { get ; set ; }
        public DateTime Updated { get; set; }
    }
}
