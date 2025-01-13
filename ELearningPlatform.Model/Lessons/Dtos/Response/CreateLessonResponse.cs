

namespace ELearningPlatform.Model.Lessons.Dtos.Response;
public sealed record CreateLessonResponse(int Id,Guid CourseId, string Title, string Description, DateTime Created);    





