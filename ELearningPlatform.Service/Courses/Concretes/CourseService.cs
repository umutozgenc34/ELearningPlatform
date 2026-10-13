using AutoMapper;
using Core.Infrastructures.CloudinaryServices;
using Core.Response;
using ELearningPlatform.Model.Courses.Dtos.Request;
using ELearningPlatform.Model.Courses.Dtos.Response;
using ELearningPlatform.Model.Courses.Entities;
using ELearningPlatform.Model.Lessons.Dtos.Response;
using ELearningPlatform.Repository.Categories.Abstracts;
using ELearningPlatform.Repository.Courses.Abstracts;
using ELearningPlatform.Repository.UnitOfWorks.Abstracts;
using ELearningPlatform.Service.Courses.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ELearningPlatform.Service.Courses.Concretes;

public class CourseService(ICourseRepository courseRepository, ICategoryRepository categoryRepository, IMapper mapper, IUnitOfWork unitOfWork,
                            ICloudinaryService cloudinaryService, ILogger<CourseService> logger) : ICourseService
{
    public async Task<ServiceResult<CreateCourseResponse>> CreateAsync(CreateCourseRequest request)
    {
        logger.LogInformation("CreateAsync called for course: {Name}", request.Name);

        var category = await categoryRepository.GetByIdAsync(request.CategoryId);
        if (category is null)
        {
            logger.LogWarning("CreateAsync failed. Category not found. CategoryId: {CategoryId}", request.CategoryId);
            return ServiceResult<CreateCourseResponse>.Fail("Kategori bulunamadı.", HttpStatusCode.NotFound);
        }

        string imageUrl = string.Empty;
        if (request.ImageFile != null)
        {
            imageUrl = await cloudinaryService.UploadImage(request.ImageFile, "course-images");
            logger.LogInformation("Image uploaded successfully for course: {Name}", request.Name);
        }

        var course = mapper.Map<Course>(request);
        course.ImageUrl = imageUrl;
        course.Category = category;
        course.CourseDetails ??= new CourseDetails { EducatorFullName = "Bilinmeyen Eğitmen" };

        await courseRepository.AddAsync(course);
        await unitOfWork.SaveChangesAsync();

        var responseAsDto = mapper.Map<CreateCourseResponse>(course);

        logger.LogInformation("Course created successfully. ID: {Id}", responseAsDto.Id);
        return ServiceResult<CreateCourseResponse>.SuccessAsCreated(responseAsDto, $"api/v1/courses/{responseAsDto.Id}");
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

        // FindAsync yerine sorgu kullanıyoruz ki Category ve CourseDetails (AutoInclude) da gelsin.
        var course = await courseRepository.GetAll().FirstOrDefaultAsync(c => c.Id == id);
        if (course is null)
        {
            logger.LogWarning("GetByIdAsync failed. Course not found. ID: {Id}", id);
            return ServiceResult<CourseDto>.Fail("Kurs bulunamadı.", HttpStatusCode.NotFound);
        }

        var courseAsDto = mapper.Map<CourseDto>(course);
        logger.LogInformation("GetByIdAsync succeeded for course ID: {Id}", id);
        return ServiceResult<CourseDto>.Success(courseAsDto);
    }

    public async Task<ServiceResult<List<CourseDto>>> GetCoursesByDescriptionKeyword(string keyword)
    {
        logger.LogInformation("GetCoursesByDescriptionKeyword called. Keyword: {Keyword}", keyword);

        var courses = string.IsNullOrWhiteSpace(keyword)
            ? await courseRepository.GetAll().ToListAsync()
            : await courseRepository.GetCoursesByDescriptionKeyword(keyword.Trim()).ToListAsync();

        // Önceden entity'ler doğrudan dönüyordu; artık diğer uç noktalarla aynı DTO dönüyor.
        var coursesAsDto = mapper.Map<List<CourseDto>>(courses);

        logger.LogInformation("GetCoursesByDescriptionKeyword returned {Count} courses.", coursesAsDto.Count);
        return ServiceResult<List<CourseDto>>.Success(coursesAsDto);
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

        // Not: Burada önceden "Id'si farklı herhangi bir kurs var mı?" kontrolü vardı.
        // Veritabanında başka bir kurs olduğu sürece bu hep true döndüğü için güncelleme hiç çalışmıyordu.
        var course = await courseRepository.GetByIdAsync(request.Id);
        if (course is null)
        {
            logger.LogWarning("UpdateAsync failed. Course not found. ID: {Id}", request.Id);
            return ServiceResult.Fail("Kurs bulunamadı.", HttpStatusCode.NotFound);
        }

        var category = await categoryRepository.GetByIdAsync(request.CategoryId);
        if (category is null)
        {
            logger.LogWarning("UpdateAsync failed. Category not found. CategoryId: {CategoryId}", request.CategoryId);
            return ServiceResult.Fail("Kategori bulunamadı.", HttpStatusCode.NotFound);
        }

        mapper.Map(request, course);

        if (request.ImageFile != null)
        {
            course.ImageUrl = await cloudinaryService.UploadImage(request.ImageFile, "course-images");
            logger.LogInformation("New image uploaded for course ID: {Id}", request.Id);
        }

        courseRepository.Update(course);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Course updated successfully. ID: {Id}", request.Id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CourseDto>>> GetCoursesByCategoryIdAsync(int categoryId)
    {
        logger.LogInformation("GetCoursesByCategoryIdAsync called for category ID: {CategoryId}", categoryId);

        var courses = await courseRepository
            .GetAll()
            .Where(c => c.CategoryId == categoryId)
            .ToListAsync();

        // Kategoride kurs yoksa hata değil boş liste dönüyoruz; frontend "henüz kurs yok" gösterebilsin.
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

        var lessonsDto = mapper.Map<List<LessonDto>>(course.Lessons.OrderBy(l => l.LessonOrder));
        logger.LogInformation("{Count} lessons found for course ID: {CourseId}", lessonsDto.Count, courseId);
        return ServiceResult<List<LessonDto>>.Success(lessonsDto);
    }
}
