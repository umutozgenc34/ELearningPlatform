using Core.Constants;
using ELearningPlatform.Model.Lessons.Dtos.Request;
using ELearningPlatform.Service.Lessons.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;

public class LessonsController(ILessonService lessonService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetLessons() => CreateActionResult(await lessonService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetLesson([FromRoute] int id) => CreateActionResult(await lessonService.GetByIdAsync(id));

    [Authorize(Roles = AppRoles.AdminOrEducator)]
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateLesson([FromForm] CreateLessonRequest request) =>
         CreateActionResult(await lessonService.CreateAsync(request));

    [Authorize(Roles = AppRoles.AdminOrEducator)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateLesson([FromRoute] int id, [FromBody] UpdateLessonRequest request) =>
        CreateActionResult(await lessonService.UpdateAsync(request with { Id = id }));

    [Authorize(Roles = AppRoles.AdminOrEducator)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteLesson([FromRoute] int id) => CreateActionResult(await lessonService.DeleteAsync(id));
}
