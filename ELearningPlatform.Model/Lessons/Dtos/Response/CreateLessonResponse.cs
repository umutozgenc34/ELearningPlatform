

namespace ELearningPlatform.Model.Lessons.Dtos.Response;
public sealed class CreateLessonResponse
{
    public int Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string VideoUrl { get; set; }
    public DateTime Created { get; set; }


    public CreateLessonResponse()
    {
    }


    public CreateLessonResponse(int id, Guid courseId, string title, string description,string videoUrl, DateTime created)
    {
        Id = id;
        CourseId = courseId;
        Title = title;
        Description = description;
        VideoUrl = videoUrl;
        Created = created;
    }
}




