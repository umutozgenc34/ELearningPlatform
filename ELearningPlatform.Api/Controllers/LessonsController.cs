using ELearningPlatform.Model.Lessons.Dtos.Request;
using ELearningPlatform.Service.Lessons.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;


public class LessonsController(ILessonService lessonService) : CustomBaseController
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetLessons() => CreateActionResult(await lessonService.GetAllAsync());

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLesson(int id) => CreateActionResult(await lessonService.GetByIdAsync(id));

    [Authorize(Roles = "Educator,Admin")]
    [HttpPost]
    [Consumes("multipart/form-data")] 
    public async Task<IActionResult> CreateLesson([FromForm] CreateLessonRequest request) =>
         CreateActionResult(await lessonService.CreateAsync(request));

    [Authorize(Roles = "Educator,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLesson(UpdateLessonRequest request) => CreateActionResult(await lessonService.UpdateAsync(request));

    [Authorize(Roles = "Educator,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLesson(int id) => CreateActionResult(await lessonService.DeleteAsync(id));
}

