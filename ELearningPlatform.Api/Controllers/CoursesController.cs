using ELearningPlatform.Model.Courses.Dtos.Request;
using ELearningPlatform.Service.Courses.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;

public class CoursesController(ICourseService courseService) : CustomBaseController
{
    //[HttpGet]
    //public async Task<IActionResult> GetCourses() => CreateActionResult(await courseService.GetAllAsync());
    //[HttpGet("{id}")]
    //public async Task<IActionResult> GetCourse(Guid id) => CreateActionResult(await courseService.GetByIdAsync(id));

    [HttpGet("details")]
    public async Task<IActionResult> GetCoursesWithDetails() => CreateActionResult(await courseService.GetAllCoursesWithDetailsAsync());
    [HttpGet("{id:guid}/details")]
    public async Task<IActionResult> GetCourseWithDetails(Guid id) => CreateActionResult(await courseService.GetCourseWithDetailsAsync(id));

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateCourse([FromForm] CreateCourseRequest request) => CreateActionResult(await courseService
        .CreateAsync(request));
    [HttpPut]
    public async Task<IActionResult> UpdateCourse(UpdateCourseRequest request) => CreateActionResult
        (await courseService.UpdateAsync(request));
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCourse(Guid id) => CreateActionResult(await courseService.DeleteAsync(id));
}
