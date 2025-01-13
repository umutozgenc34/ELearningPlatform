

namespace ELearningPlatform.Model.Lessons.Dtos.Response;
public sealed record LessonDto(int Id, Guid CourseId, string Title, string VideoUrl, int LessonOrder, DateTime Created, DateTime Updated);  
