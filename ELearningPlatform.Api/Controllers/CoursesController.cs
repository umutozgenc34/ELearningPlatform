using Core.Constants;
using ELearningPlatform.Model.Courses.Dtos.Request;
using ELearningPlatform.Service.Courses.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;

public class CoursesController(ICourseService courseService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetCourses() => CreateActionResult(await courseService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCourse([FromRoute] Guid id) => CreateActionResult(await courseService.GetByIdAsync(id));

    [HttpGet("details")]
    public async Task<IActionResult> GetCoursesWithDetails() => CreateActionResult(await courseService.GetAllCoursesWithDetailsAsync());

    [HttpGet("{id:guid}/details")]
    public async Task<IActionResult> GetCourseWithDetails([FromRoute] Guid id) => CreateActionResult(await courseService.GetCourseWithDetailsAsync(id));

    [HttpGet("search")]
    public async Task<IActionResult> SearchCourses([FromQuery] string? keyword) =>
        CreateActionResult(await courseService.GetCoursesByDescriptionKeyword(keyword ?? string.Empty));

    [HttpGet("byCategory")]
    public async Task<IActionResult> GetCoursesByCategoryId([FromQuery] int categoryId) => CreateActionResult(await courseService.GetCoursesByCategoryIdAsync(categoryId));

    [HttpGet("{courseId:guid}/lessons")]
    public async Task<IActionResult> GetLessonsByCourseId([FromRoute] Guid courseId) => CreateActionResult(await courseService.GetLessonsByCourseIdAsync(courseId));

    [Authorize(Roles = AppRoles.AdminOrEducator)]
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateCourse([FromForm] CreateCourseRequest request) => CreateActionResult(await courseService
        .CreateAsync(request));

    [Authorize(Roles = AppRoles.AdminOrEducator)]
    [HttpPut]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateCourse([FromForm] UpdateCourseRequest request) => CreateActionResult
        (await courseService.UpdateAsync(request));

    [Authorize(Roles = AppRoles.AdminOrEducator)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCourse([FromRoute] Guid id) => CreateActionResult(await courseService.DeleteAsync(id));
}
