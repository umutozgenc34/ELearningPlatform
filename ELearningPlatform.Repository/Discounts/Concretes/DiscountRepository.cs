using Core.Repositories.Concretes;
using ELearningPlatform.Model.Discounts.Entity;
using ELearningPlatform.Repository.Contexts;
using ELearningPlatform.Repository.Discounts.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace ELearningPlatform.Repository.Discounts.Concretes;

public class DiscountRepository(AppDbContext context) : GenericRepository<AppDbContext,Discount,int>(context),IDiscountRepository
{
    public async Task<Discount> GetByCouponAsync(string coupon) => await context.Discounts.FirstOrDefaultAsync(d => d.Coupon == coupon);
}
