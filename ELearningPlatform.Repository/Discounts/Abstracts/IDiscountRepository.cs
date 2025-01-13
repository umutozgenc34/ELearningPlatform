using Core.Repositories.Abstracts;
using ELearningPlatform.Model.Discounts.Entity;

namespace ELearningPlatform.Repository.Discounts.Abstracts;

public interface IDiscountRepository : IGenericRepository<Discount,int>
{
    Task<Discount> GetByCouponAsync(string coupon);
}
