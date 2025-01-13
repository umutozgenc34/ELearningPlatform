using AutoMapper;
using Core.Response;
using ELearningPlatform.Model.Lessons.Dtos.Request;
using ELearningPlatform.Model.Lessons.Dtos.Response;
using ELearningPlatform.Model.Lessons.Entity;
using ELearningPlatform.Repository.Lessons.Abstracts;
using ELearningPlatform.Repository.UnitOfWorks.Abstracts;
using ELearningPlatform.Service.Lessons.Abstracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELearningPlatform.Service.Lessons
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LessonService(ILessonRepository lessonRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _lessonRepository = lessonRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Tüm dersleri alır
        public async Task<ServiceResult<List<LessonDto>>> GetAllAsync()
        {
            var lessons = await _lessonRepository.GetAll().ToListAsync();
            var lessonDtos = _mapper.Map<List<LessonDto>>(lessons); // Entity'leri DTO'ya dönüştürür
            return ServiceResult<List<LessonDto>>.Success(lessonDtos);
        }

        // ID'ye göre bir ders alır
        public async Task<ServiceResult<LessonDto>> GetByIdAsync(int Id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(Id);
            if (lesson == null)
            {
                return ServiceResult<LessonDto>.Fail($"Ders bulunamadı. Id: {Id}");
            }

            var lessonDto = _mapper.Map<LessonDto>(lesson);
            return ServiceResult<LessonDto>.Success(lessonDto);
        }

        // Yeni bir ders ekler
        public async Task<ServiceResult<CreateLessonResponse>> CreateAsync(CreateLessonRequest request)
        {
            var lesson = _mapper.Map<Lesson>(request);
            lesson.Created = DateTime.UtcNow;
            lesson.Updated = DateTime.UtcNow;

            await _lessonRepository.AddAsync(lesson);
            await _unitOfWork.SaveChangesAsync();

            var lessonResponse = _mapper.Map<CreateLessonResponse>(lesson);
            return ServiceResult<CreateLessonResponse>.Success(lessonResponse);
        }

        // Mevcut bir dersi günceller
        public async Task<ServiceResult> UpdateAsync(UpdateLessonRequest request)
        {
            var lesson = await _lessonRepository.GetByIdAsync(request.Id);
            if (lesson == null)
            {
                return ServiceResult.Fail($"Ders bulunamadı. Id: {request.Id}");
            }

            lesson.Title = request.Title;
            lesson.VideoUrl = request.VideoUrl;
            lesson.LessonOrder = request.LessonOrder;
            lesson.Updated = DateTime.UtcNow;

            _lessonRepository.Update(lesson);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Success();
        }

        // Bir dersi siler
        public async Task<ServiceResult> DeleteAsync(int Id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(Id);
            if (lesson == null)
            {
                return ServiceResult.Fail($"Ders silinemedi. Id: {Id} ile eşleşen bir ders bulunamadı.");
            }

            _lessonRepository.Delete(lesson);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Success();
        }
    }
}
