namespace ELearningPlatform.Model.Courses.Entities;

public sealed class CourseDetails
{
    public Guid CourseId { get; set; } // pk
    public int Duration { get; set; }
    public float Rating { get; set; }
    public string EducatorFullName { get; set; } = default!;
}
