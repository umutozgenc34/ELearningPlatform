using AutoMapper;
using ELearningPlatform.Model.Lessons.Dtos.Request;
using ELearningPlatform.Model.Lessons.Dtos.Response;
using ELearningPlatform.Model.Lessons.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningPlatform.Service.Lessons.Profiles
{
    public class LessonMappingProfile : Profile
    {
        public LessonMappingProfile()
        {
            // Request ve Entity arasındaki dönüşüm
            CreateMap<CreateLessonRequest, Lesson>();
            CreateMap<UpdateLessonRequest, Lesson>();

            // Entity ve Response arasındaki dönüşüm
            CreateMap<Lesson, CreateLessonResponse>();

            // Entity ve DTO arasındaki dönüşüm
            CreateMap<Lesson, LessonDto>().ReverseMap();
        }
    }
} 
