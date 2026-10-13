using Core.Constants;
using ELearningPlatform.Model.Categories.Dtos.Request;
using ELearningPlatform.Service.Categories.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;

public class CategoriesController(ICategoryService categoryService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetCategories() => CreateActionResult(await categoryService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategory([FromRoute] int id) => CreateActionResult(await categoryService.GetByIdAsync(id));

    [Authorize(Roles = AppRoles.AdminOrEducator)]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request) => CreateActionResult(await categoryService
        .CreateAsync(request));

    // Route'taki id esas alınır; body'deki Id ile çakışmasın diye route'takiyle değiştirilir.
    [Authorize(Roles = AppRoles.AdminOrEducator)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] UpdateCategoryRequest request) => CreateActionResult
        (await categoryService.UpdateAsync(request with { Id = id }));

    [Authorize(Roles = AppRoles.AdminOrEducator)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] int id) => CreateActionResult(await categoryService.DeleteAsync(id));
}
