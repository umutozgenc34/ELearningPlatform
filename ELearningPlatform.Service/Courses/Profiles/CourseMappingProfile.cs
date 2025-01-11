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

       
        CreateMap<CreateCourseRequest, CreateCourseResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryId))  
            .ForMember(dest => dest.CourseDetails, opt => opt.MapFrom(src => src.CourseDetails));

       
        CreateMap<UpdateCourseRequest, Category>();
        CreateMap<CourseDetails, CourseDetailsDto>().ReverseMap();
    }
}

