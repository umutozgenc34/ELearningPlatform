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

        
        CreateMap<CreateCourseResponse, Course>();
        CreateMap<Course, CreateCourseResponse>();
        CreateMap<UpdateCourseRequest, Category>();
        CreateMap<CourseDetails, CourseDetailsDto>().ReverseMap();
    }
}
