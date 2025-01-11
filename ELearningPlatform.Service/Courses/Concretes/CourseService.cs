using AutoMapper;
using Core.Response;
using ELearningPlatform.Model.Courses.Dtos.Request;
using ELearningPlatform.Model.Courses.Dtos.Response;
using ELearningPlatform.Model.Courses.Entities;
using ELearningPlatform.Repository.Courses.Abstracts;
using ELearningPlatform.Repository.UnitOfWorks.Abstracts;
using ELearningPlatform.Service.Courses.Abstracts;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ELearningPlatform.Service.Courses.Concretes;

public class CourseService(ICourseRepository courseRepository,IMapper mapper,IUnitOfWork unitOfWork) : ICourseService
{
    public async Task<ServiceResult<CreateCourseResponse>> CreateAsync(CreateCourseRequest request)
    {
        var course = mapper.Map<Course>(request);

        await courseRepository.AddAsync(course);
        await unitOfWork.SaveChangesAsync();

        var responseAsDto = mapper.Map<CreateCourseResponse>(course);
        return ServiceResult<CreateCourseResponse>.SuccessAsCreated(responseAsDto, $"api/courses/{responseAsDto.Id}");
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        var course = await courseRepository.GetByIdAsync(id);
        if (course is null)
        {
            return ServiceResult.Fail("Kurs bulunamadı.",HttpStatusCode.NotFound);
        }

        courseRepository.Delete(course);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CourseDto>>> GetAllAsync()
    {
        var courses = await courseRepository.GetAll().ToListAsync();

        var coursesAsDto = mapper.Map<List<CourseDto>>(courses);

        return ServiceResult<List<CourseDto>>.Success(coursesAsDto);
    }

    public async Task<ServiceResult<List<CourseDto>>> GetAllCoursesWithDetailsAsync()
    {
        var courses = await courseRepository.GetAllCoursesWithDetailsAsync();

        var coursesAsDto = mapper.Map<List<CourseDto>>(courses);

        return ServiceResult<List<CourseDto>>.Success(coursesAsDto);

    }

    public async Task<ServiceResult<CourseDto>> GetByIdAsync(Guid id)
    {
        var course = await courseRepository.GetByIdAsync(id);
        if(course is null)
        {
            return ServiceResult<CourseDto>.Fail("Kurs bulunamadı.",HttpStatusCode.NotFound);
        }

        var courseAsDto = mapper.Map<CourseDto>(course);

        return ServiceResult<CourseDto>.Success(courseAsDto);

    }

    public async Task<ServiceResult<CourseDto>> GetCourseWithDetailsAsync(Guid id)
    {
        var course = await courseRepository.GetCourseWithDetailsAsync(id);
        if (course is null)
        {
            ServiceResult<CourseDto>.Fail("Kurs bulunamadı.",HttpStatusCode.NotFound);
        }

        var courseAsDto = mapper.Map<CourseDto>(course);
        return ServiceResult<CourseDto>.Success(courseAsDto);
    }

    public async Task<ServiceResult> UpdateAsync(UpdateCourseRequest request)
    {
        var isCourseIdExist = await courseRepository.Where(x => x.Id != request.Id).AnyAsync();

        if (isCourseIdExist is true)
        {
            return ServiceResult.Fail("Aynı Id'ye sahip bir kurs var", HttpStatusCode.BadRequest);
        }

        var course = await courseRepository.GetByIdAsync(request.Id);
        if (course is null)
        {
            return ServiceResult.Fail("Course bulunamadı.", HttpStatusCode.NotFound);
        }

        var courseAsDto = mapper.Map(request, course);

        courseRepository.Update(course);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
