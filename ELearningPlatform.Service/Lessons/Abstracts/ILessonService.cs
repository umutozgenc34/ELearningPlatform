using Core.Response;
using ELearningPlatform.Model.Lessons.Dtos.Request;
using ELearningPlatform.Model.Lessons.Dtos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningPlatform.Service.Lessons.Abstracts
{
    public interface ILessonService
    {
        
        Task<ServiceResult<List<LessonDto>>> GetAllAsync();  // Tüm dersleri alır
        Task<ServiceResult<LessonDto>> GetByIdAsync(int id);  // ID'ye göre bir ders alır
        Task<ServiceResult<CreateLessonResponse>> CreateAsync(CreateLessonRequest request);  // Yeni bir ders ekler
        Task<ServiceResult> UpdateAsync(UpdateLessonRequest request);  // Mevcut bir dersi günceller
        Task<ServiceResult> DeleteAsync(int id);  // Bir dersi siler
    }
}

