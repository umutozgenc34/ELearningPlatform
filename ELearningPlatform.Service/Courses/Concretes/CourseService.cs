using AutoMapper;
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
using Microsoft.Extensions.Logging;
using System.Net;

namespace ELearningPlatform.Service.Courses.Concretes;

public class CourseService(ICourseRepository courseRepository,IMapper mapper,IUnitOfWork unitOfWork,
                            ICloudinaryService cloudinaryService, ILogger<CourseService> logger) : ICourseService
{
    public async Task<ServiceResult<CreateCourseResponse>> CreateAsync(CreateCourseRequest request)
    {
        logger.LogInformation("CreateAsync called for course: {Name}", request.Name);

        string imageUrl = string.Empty;
        if (request.ImageFile != null)
        {
            imageUrl = await cloudinaryService.UploadImage(request.ImageFile, "course-images");
            logger.LogInformation("Image uploaded successfully for course: {Name}", request.Name);
        }

        var course = mapper.Map<Course>(request);
        course.ImageUrl = imageUrl;

        await courseRepository.AddAsync(course);
        await unitOfWork.SaveChangesAsync();

        var responseAsDto = mapper.Map<CreateCourseResponse>(course);

        logger.LogInformation("Course created successfully. ID: {Id}", responseAsDto.Id);
        return ServiceResult<CreateCourseResponse>.SuccessAsCreated(responseAsDto, $"api/courses/{responseAsDto.Id}");
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        logger.LogInformation("DeleteAsync called for course ID: {Id}", id);

        var course = await courseRepository.GetByIdAsync(id);
        if (course is null)
        {
            logger.LogWarning("DeleteAsync failed. Course not found. ID: {Id}", id);
            return ServiceResult.Fail("Kurs bulunamadı.", HttpStatusCode.NotFound);
        }

        courseRepository.Delete(course);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Course deleted successfully. ID: {Id}", id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CourseDto>>> GetAllAsync()
    {
        logger.LogInformation("GetAllAsync called.");

        var courses = await courseRepository.GetAll().ToListAsync();
        var coursesAsDto = mapper.Map<List<CourseDto>>(courses);

        logger.LogInformation("GetAllAsync returned {Count} courses.", coursesAsDto.Count);
        return ServiceResult<List<CourseDto>>.Success(coursesAsDto);
    }

    public async Task<ServiceResult<List<CourseDto>>> GetAllCoursesWithDetailsAsync()
    {
        logger.LogInformation("GetAllCoursesWithDetailsAsync called.");

        var courses = await courseRepository.GetAllCoursesWithDetailsAsync();
        var coursesAsDto = mapper.Map<List<CourseDto>>(courses);

        logger.LogInformation("GetAllCoursesWithDetailsAsync returned {Count} courses.", coursesAsDto.Count);
        return ServiceResult<List<CourseDto>>.Success(coursesAsDto);
    }

    public async Task<ServiceResult<CourseDto>> GetByIdAsync(Guid id)
    {
        logger.LogInformation("GetByIdAsync called for course ID: {Id}", id);

        var course = await courseRepository.GetByIdAsync(id);
        if (course is null)
        {
            logger.LogWarning("GetByIdAsync failed. Course not found. ID: {Id}", id);
            return ServiceResult<CourseDto>.Fail("Kurs bulunamadı.", HttpStatusCode.NotFound);
        }

        var courseAsDto = mapper.Map<CourseDto>(course);
        logger.LogInformation("GetByIdAsync succeeded for course ID: {Id}", id);
        return ServiceResult<CourseDto>.Success(courseAsDto);
    }

    public async Task<ServiceResult<List<Course>>> GetCoursesByDescriptionKeyword(string keyword)
    {
        logger.LogInformation("GetCoursesByDescriptionKeyword called. Keyword: {Keyword}", keyword);

        var courses = await courseRepository.GetCoursesByDescriptionKeyword(keyword).ToListAsync();

        logger.LogInformation("GetCoursesByDescriptionKeyword returned {Count} courses.", courses.Count);
        return ServiceResult<List<Course>>.Success(courses);
    }

    public async Task<ServiceResult<CourseDto>> GetCourseWithDetailsAsync(Guid id)
    {
        logger.LogInformation("GetCourseWithDetailsAsync called for course ID: {Id}", id);

        var course = await courseRepository.GetCourseWithDetailsAsync(id);
        if (course is null)
        {
            logger.LogWarning("GetCourseWithDetailsAsync failed. Course not found. ID: {Id}", id);
            return ServiceResult<CourseDto>.Fail("Kurs bulunamadı.", HttpStatusCode.NotFound);
        }

        var courseAsDto = mapper.Map<CourseDto>(course);
        logger.LogInformation("GetCourseWithDetailsAsync succeeded for course ID: {Id}", id);
        return ServiceResult<CourseDto>.Success(courseAsDto);
    }

    public async Task<ServiceResult> UpdateAsync(UpdateCourseRequest request)
    {
        logger.LogInformation("UpdateAsync called for course ID: {Id}", request.Id);

        var isCourseIdExist = await courseRepository
            .Where(x => x.Id != request.Id)
            .AnyAsync();

        if (isCourseIdExist)
        {
            logger.LogWarning("UpdateAsync failed. A different course with ID: {Id} already exists.", request.Id);
            return ServiceResult.Fail("Aynı Id'ye sahip bir kurs var", HttpStatusCode.BadRequest);
        }

        var course = await courseRepository.GetByIdAsync(request.Id);
        if (course is null)
        {
            logger.LogWarning("UpdateAsync failed. Course not found. ID: {Id}", request.Id);
            return ServiceResult.Fail("Course bulunamadı.", HttpStatusCode.NotFound);
        }

        if (request.ImageFile != null)
        {
            string imageUrl = await cloudinaryService.UploadImage(request.ImageFile, "course-images");
            course.ImageUrl = imageUrl;
            logger.LogInformation("New image uploaded for course ID: {Id}", request.Id);
        }

        mapper.Map(request, course);
        courseRepository.Update(course);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Course updated successfully. ID: {Id}", request.Id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CourseDto>>> GetCoursesByCategoryIdAsync(int categoryId)
    {
        logger.LogInformation("GetCoursesByCategoryIdAsync called for category ID: {CategoryId}", categoryId);

        var courses = await courseRepository
            .Where(c => c.CategoryId == categoryId)
            .ToListAsync();

        if (!courses.Any())
        {
            logger.LogWarning("No courses found for category ID: {CategoryId}", categoryId);
            return ServiceResult<List<CourseDto>>.Fail("Bu kategoriye ait kurs bulunamadı.", HttpStatusCode.NotFound);
        }

        var coursesAsDto = mapper.Map<List<CourseDto>>(courses);

        logger.LogInformation("{Count} course(s) found for category ID: {CategoryId}", coursesAsDto.Count, categoryId);
        return ServiceResult<List<CourseDto>>.Success(coursesAsDto);
    }

    public async Task<ServiceResult<List<LessonDto>>> GetLessonsByCourseIdAsync(Guid courseId)
    {
        logger.LogInformation("GetLessonsByCourseIdAsync called for course ID: {CourseId}", courseId);

        var course = await courseRepository.GetCourseWithLessonsAsync(courseId);

        if (course is null)
        {
            logger.LogWarning("Course not found for ID: {CourseId}", courseId);
            return ServiceResult<List<LessonDto>>.Fail("Kurs bulunamadı.", HttpStatusCode.NotFound);
        }

        var lessonsDto = mapper.Map<List<LessonDto>>(course.Lessons);
        logger.LogInformation("{Count} lessons found for course ID: {CourseId}", lessonsDto.Count, courseId);
        return ServiceResult<List<LessonDto>>.Success(lessonsDto);
    }

}
