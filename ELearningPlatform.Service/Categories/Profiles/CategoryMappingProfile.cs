using AutoMapper;
using ELearningPlatform.Model.Categories.Dtos.Request;
using ELearningPlatform.Model.Categories.Dtos.Response;
using ELearningPlatform.Model.Categories.Entity;

namespace ELearningPlatform.Service.Categories.Profiles;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<CreateCategoryRequest, Category>();
        CreateMap<CategoryDto, Category>().ReverseMap();
        CreateMap<Category, CreateCategoryResponse>();
        CreateMap<UpdateCategoryRequest, Category>();
    }
}
