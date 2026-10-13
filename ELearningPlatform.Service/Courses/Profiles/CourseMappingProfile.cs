using AutoMapper;
using ELearningPlatform.Model.Categories.Entity;
using ELearningPlatform.Model.Courses.Dtos.Request;
using ELearningPlatform.Model.Courses.Dtos.Response;
using ELearningPlatform.Model.Courses.Entities;

namespace ELearningPlatform.Service.Courses.Profiles;

public class CourseMappingProfile : Profile
{
    public CourseMappingProfile()
    {
        
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ReverseMap();

        CreateMap<CreateCourseRequest, Course>();

        CreateMap<Course, CreateCourseResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name)) 
            .ForMember(dest => dest.CourseDetails, opt => opt.MapFrom(src => src.CourseDetails));

        CreateMap<UpdateCourseRequest, Course>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.CourseDetails, opt => opt.Ignore())
            .ForMember(dest => dest.Lessons, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
        CreateMap<CourseDetails, CourseDetailsDto>().ReverseMap();
    }
}

