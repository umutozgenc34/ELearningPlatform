using AutoMapper;
using ELearningPlatform.Model.Discounts.Dtos;
using ELearningPlatform.Model.Discounts.Entity;

namespace ELearningPlatform.Service.Discounts.Profiles;

public class DiscountMappingProfile : Profile
{
    public DiscountMappingProfile()
    {
        CreateMap<Discount, DiscountDto>().ReverseMap();
    }
}
