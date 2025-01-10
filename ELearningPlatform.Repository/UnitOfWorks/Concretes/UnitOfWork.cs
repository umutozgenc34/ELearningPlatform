using ELearningPlatform.Repository.Contexts;
using ELearningPlatform.Repository.UnitOfWorks.Abstracts;

namespace ELearningPlatform.Repository.UnitOfWorks.Concretes;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
}
