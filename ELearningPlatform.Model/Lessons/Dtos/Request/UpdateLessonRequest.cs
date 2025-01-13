

namespace ELearningPlatform.Model.Lessons.Dtos.Request;
public sealed record UpdateLessonRequest(int Id,Guid CourseId, string Title, string VideoUrl, int LessonOrder);   

