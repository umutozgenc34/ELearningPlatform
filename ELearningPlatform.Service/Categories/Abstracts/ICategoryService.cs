using Core.Response;
using ELearningPlatform.Model.Categories.Dtos.Request;
using ELearningPlatform.Model.Categories.Dtos.Response;

namespace ELearningPlatform.Service.Categories.Abstracts;

public interface ICategoryService
{
    Task<ServiceResult<List<CategoryDto>>> GetAllAsync();
    Task<ServiceResult<CategoryDto>> GetByIdAsync(int id);
    Task<ServiceResult<CreateCategoryResponse>> CreateAsync(CreateCategoryRequest request);
    Task<ServiceResult> UpdateAsync(UpdateCategoryRequest request);
    Task<ServiceResult> DeleteAsync(int id);

}
