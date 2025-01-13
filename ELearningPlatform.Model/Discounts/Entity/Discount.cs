using Core.Models;

namespace ELearningPlatform.Model.Discounts.Entity;

public class Discount : BaseEntity<int>
{
    public string Coupon { get; set; } = default!;
    public decimal Rate { get; set; } //indirim yüzdesi
}
