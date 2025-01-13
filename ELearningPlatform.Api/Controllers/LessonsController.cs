using ELearningPlatform.Model.Lessons.Dtos.Request;
using ELearningPlatform.Model.Lessons.Dtos.Response;
using ELearningPlatform.Service.Lessons.Abstracts;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ELearningPlatform.Api.Controllers
{

    public class LessonsController(ILessonService lessonService) : CustomBaseController
    {
      
    [HttpGet]
        public async Task<IActionResult> GetLessons() => CreateActionResult(await lessonService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLesson(int id) => CreateActionResult(await lessonService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> CreateLesson(CreateLessonRequest request) => CreateActionResult(await lessonService.CreateAsync(request));

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLesson(UpdateLessonRequest request) => CreateActionResult(await lessonService.UpdateAsync(request));

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLesson(int id) => CreateActionResult(await lessonService.DeleteAsync(id));
    }
}

