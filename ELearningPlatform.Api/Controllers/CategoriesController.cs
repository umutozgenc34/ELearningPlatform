using ELearningPlatform.Model.Categories.Dtos.Request;
using ELearningPlatform.Model.Categories.Entity;
using ELearningPlatform.Service.Categories.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;

public class CategoriesController(ICategoryService categoryService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetCategories() => CreateActionResult(await categoryService.GetAllAsync());
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory([FromRoute] int id) => CreateActionResult(await categoryService.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request) => CreateActionResult(await categoryService
        .CreateAsync(request));
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory([FromBody]UpdateCategoryRequest request) => CreateActionResult
        (await categoryService.UpdateAsync(request));
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] int id) => CreateActionResult(await categoryService.DeleteAsync(id));

}
