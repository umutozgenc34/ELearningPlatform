


namespace ELearningPlatform.Model.Lessons.Dtos.Request;

public record CreateLessonRequest(Guid CourseId,string Title,string VideoUrl, int LessonOrder);

