

using Core.Repositories.Abstracts;
using ELearningPlatform.Model.Lessons.Entity;

namespace ELearningPlatform.Repository.Lessons.Abstracts;
public interface ILessonRepository : IGenericRepository<Lesson, int>;