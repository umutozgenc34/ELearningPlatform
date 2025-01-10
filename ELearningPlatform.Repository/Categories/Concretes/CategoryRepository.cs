using Core.Repositories.Concretes;
using ELearningPlatform.Model.Categories.Entity;
using ELearningPlatform.Repository.Categories.Abstracts;
using ELearningPlatform.Repository.Contexts;

namespace ELearningPlatform.Repository.Categories.Concretes;

public class CategoryRepository(AppDbContext context) : GenericRepository<AppDbContext,Category,int>(context),ICategoryRepository
{
}
