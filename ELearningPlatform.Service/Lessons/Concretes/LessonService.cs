using AutoMapper;
using Core.Infrastructures.CloudinaryServices;
using Core.Response;
using ELearningPlatform.Model.Lessons.Dtos.Request;
using ELearningPlatform.Model.Lessons.Dtos.Response;
using ELearningPlatform.Model.Lessons.Entity;
using ELearningPlatform.Repository.Courses.Abstracts;
using ELearningPlatform.Repository.Lessons.Abstracts;
using ELearningPlatform.Repository.UnitOfWorks.Abstracts;
using ELearningPlatform.Service.Lessons.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ELearningPlatform.Service.Lessons
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<LessonService> _logger;

        public LessonService(ILessonRepository lessonRepository, ICourseRepository courseRepository, IUnitOfWork unitOfWork,
            IMapper mapper, ICloudinaryService cloudinaryService, ILogger<LessonService> logger)
        {
            _lessonRepository = lessonRepository;
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }
        public async Task<ServiceResult<List<LessonDto>>> GetAllAsync()
        {
            _logger.LogInformation("GetAllAsync called.");

            var lessons = await _lessonRepository.GetAll().ToListAsync();
            var lessonDtos = _mapper.Map<List<LessonDto>>(lessons);

            _logger.LogInformation("GetAllAsync returned {Count} lessons.", lessonDtos.Count);
            return ServiceResult<List<LessonDto>>.Success(lessonDtos);
        }

        public async Task<ServiceResult<LessonDto>> GetByIdAsync(int Id)
        {
            _logger.LogInformation("GetByIdAsync called for lesson ID: {Id}", Id);

            var lesson = await _lessonRepository.GetByIdAsync(Id);
            if (lesson == null)
            {
                _logger.LogWarning("Lesson not found. ID: {Id}", Id);
                return ServiceResult<LessonDto>.Fail($"Ders bulunamadı. Id: {Id}", HttpStatusCode.NotFound);
            }

            var lessonDto = _mapper.Map<LessonDto>(lesson);
            _logger.LogInformation("Lesson found. ID: {Id}", Id);
            return ServiceResult<LessonDto>.Success(lessonDto);
        }

        public async Task<ServiceResult<CreateLessonResponse>> CreateAsync(CreateLessonRequest request)
        {
            _logger.LogInformation("CreateAsync called for lesson: {Title}", request.Title);

            // Veritabanında Lessons -> Courses yabancı anahtarı olmadığı için kursun varlığını burada kontrol ediyoruz.
            if (await _courseRepository.GetByIdAsync(request.CourseId) is null)
            {
                _logger.LogWarning("CreateAsync failed. Course not found. CourseId: {CourseId}", request.CourseId);
                return ServiceResult<CreateLessonResponse>.Fail("Kurs bulunamadı.", HttpStatusCode.NotFound);
            }

            string videoUrl = string.Empty;

            if (request.VideoFile != null)
            {
                videoUrl = await _cloudinaryService.UploadVideo(request.VideoFile, "lessons/videos");
                _logger.LogInformation("Video uploaded successfully for lesson: {Title}", request.Title);
            }

            var lesson = _mapper.Map<Lesson>(request);
            lesson.VideoUrl = videoUrl;
            lesson.Created = DateTime.UtcNow;
            lesson.Updated = DateTime.UtcNow;

            await _lessonRepository.AddAsync(lesson);
            await _unitOfWork.SaveChangesAsync();

            var lessonResponse = _mapper.Map<CreateLessonResponse>(lesson);
            _logger.LogInformation("Lesson created successfully. ID: {Id}", lesson.Id);

            return ServiceResult<CreateLessonResponse>.SuccessAsCreated(lessonResponse, $"api/v1/lessons/{lesson.Id}");
        }

        public async Task<ServiceResult> UpdateAsync(UpdateLessonRequest request)
        {
            _logger.LogInformation("UpdateAsync called for lesson ID: {Id}", request.Id);

            var lesson = await _lessonRepository.GetByIdAsync(request.Id);
            if (lesson == null)
            {
                _logger.LogWarning("UpdateAsync failed. Lesson not found. ID: {Id}", request.Id);
                return ServiceResult.Fail($"Ders bulunamadı. Id: {request.Id}", HttpStatusCode.NotFound);
            }

            if (lesson.CourseId != request.CourseId && await _courseRepository.GetByIdAsync(request.CourseId) is null)
            {
                return ServiceResult.Fail("Kurs bulunamadı.", HttpStatusCode.NotFound);
            }

            lesson.CourseId = request.CourseId;
            lesson.Title = request.Title;
            lesson.VideoUrl = request.VideoUrl;
            lesson.LessonOrder = request.LessonOrder;
            lesson.Updated = DateTime.UtcNow;

            _lessonRepository.Update(lesson);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Lesson updated successfully. ID: {Id}", request.Id);
            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> DeleteAsync(int Id)
        {
            _logger.LogInformation("DeleteAsync called for lesson ID: {Id}", Id);

            var lesson = await _lessonRepository.GetByIdAsync(Id);
            if (lesson == null)
            {
                _logger.LogWarning("DeleteAsync failed. Lesson not found. ID: {Id}", Id);
                return ServiceResult.Fail($"Ders silinemedi. Id: {Id} ile eşleşen bir ders bulunamadı.", HttpStatusCode.NotFound);
            }

            _lessonRepository.Delete(lesson);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Lesson deleted successfully. ID: {Id}", Id);
            return ServiceResult.Success(HttpStatusCode.NoContent);
        }
    }
}
