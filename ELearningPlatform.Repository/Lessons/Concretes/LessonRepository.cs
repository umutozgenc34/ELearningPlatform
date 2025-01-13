using Core.Repositories.Concretes;
using ELearningPlatform.Model.Categories.Entity;
using ELearningPlatform.Model.Lessons.Entity;
using ELearningPlatform.Repository.Categories.Abstracts;
using ELearningPlatform.Repository.Contexts;
using ELearningPlatform.Repository.Lessons.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningPlatform.Repository.Lessons.Concretes
{
    public class LessonRepository(AppDbContext context) : GenericRepository<AppDbContext, Lesson, int>(context), ILessonRepository
    {
    }
}
