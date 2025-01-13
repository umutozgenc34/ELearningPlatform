namespace ELearningPlatform.Model.Courses.Dtos.Response;

public class CourseDetailsDto
{
    public int Duration { get; set; }
    public float Rating { get; set; }
    public string EducatorFullName { get; set; } = default!;
}
