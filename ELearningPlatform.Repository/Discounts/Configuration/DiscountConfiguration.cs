using ELearningPlatform.Model.Discounts.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELearningPlatform.Repository.Discounts.Configuration;

public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.Property(x => x.Coupon).HasMaxLength(12);
        
    }
}
