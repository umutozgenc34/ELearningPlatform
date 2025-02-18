﻿using AutoMapper;
using Core.Infrastructures.CloudinaryServices;
using Core.Response;
using ELearningPlatform.Model.Courses.Dtos.Request;
using ELearningPlatform.Model.Courses.Dtos.Response;
using ELearningPlatform.Model.Courses.Entities;
using ELearningPlatform.Model.Lessons.Dtos.Response;
using ELearningPlatform.Repository.Courses.Abstracts;
using ELearningPlatform.Repository.Courses.Concretes;
using ELearningPlatform.Repository.UnitOfWorks.Abstracts;
using ELearningPlatform.Service.Courses.Abstracts;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ELearningPlatform.Service.Courses.Concretes;

public class CourseService(ICourseRepository courseRepository,IMapper mapper,IUnitOfWork unitOfWork,
                            ICloudinaryService cloudinaryService) : ICourseService
{
    public async Task<ServiceResult<CreateCourseResponse>> CreateAsync(CreateCourseRequest request)
    {
        string imageUrl = string.Empty;
        if (request.ImageFile != null)
        {
            imageUrl = await cloudinaryService.UploadImage(request.ImageFile, "course-images");
        }

        var course = mapper.Map<Course>(request);
        course.ImageUrl = imageUrl;

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

    public async Task<ServiceResult<List<Course>>> GetCoursesByDescriptionKeyword(string keyword)
    {
        var courses = await courseRepository.GetCoursesByDescriptionKeyword(keyword).ToListAsync();
        return ServiceResult<List<Course>>.Success(courses);
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

        if (request.ImageFile != null)
        {
            string imageUrl = await cloudinaryService.UploadImage(request.ImageFile, "course-images");
            course.ImageUrl = imageUrl;
        }

        var courseAsDto = mapper.Map(request, course);

        courseRepository.Update(course);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CourseDto>>> GetCoursesByCategoryIdAsync(int categoryId)
    {
        
        var courses = await courseRepository
            .Where(c => c.CategoryId == categoryId)
            .ToListAsync();

        if (!courses.Any())
        {
            return ServiceResult<List<CourseDto>>.Fail("Bu kategoriye ait kurs bulunamadı.", HttpStatusCode.NotFound);
        }

        var coursesAsDto = mapper.Map<List<CourseDto>>(courses);

        return ServiceResult<List<CourseDto>>.Success(coursesAsDto);
    }

    public async Task<ServiceResult<List<LessonDto>>> GetLessonsByCourseIdAsync(Guid courseId)
    {
        var course = await courseRepository.GetCourseWithLessonsAsync(courseId);

        if (course is null)
        {
            return ServiceResult<List<LessonDto>>.Fail("Kurs bulunamadı.", HttpStatusCode.NotFound);
        }

        var lessonsDto = mapper.Map<List<LessonDto>>(course.Lessons);
        return ServiceResult<List<LessonDto>>.Success(lessonsDto);
    }

}
