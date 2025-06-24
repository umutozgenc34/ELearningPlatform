using AutoMapper;
using Core.Response;
using ELearningPlatform.Model.Categories.Dtos.Request;
using ELearningPlatform.Model.Categories.Dtos.Response;
using ELearningPlatform.Model.Categories.Entity;
using ELearningPlatform.Repository.Categories.Abstracts;
using ELearningPlatform.Repository.UnitOfWorks.Abstracts;
using ELearningPlatform.Service.Categories.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ELearningPlatform.Service.Categories.Concretes;

public class CategoryService(ICategoryRepository categoryRepository,IMapper mapper,IUnitOfWork unitOfWork, ILogger<CategoryService> logger) : ICategoryService
{
    public async Task<ServiceResult<CreateCategoryResponse>> CreateAsync(CreateCategoryRequest request)
    {
        logger.LogInformation("CreateAsync called for category name: {CategoryName}", request.Name);

        var hasCategory = await categoryRepository.Where(x => x.Name == request.Name).AnyAsync();
        if (hasCategory)
        {
            logger.LogWarning("CreateAsync failed. Category with name '{CategoryName}' already exists.", request.Name);
            return ServiceResult<CreateCategoryResponse>.Fail("Aynı isimde bir kategori var.", HttpStatusCode.BadRequest);
        }

        var category = mapper.Map<Category>(request);
        await categoryRepository.AddAsync(category);
        await unitOfWork.SaveChangesAsync();

        var responseAsDto = mapper.Map<CreateCategoryResponse>(category);

        logger.LogInformation("Category created successfully. ID: {CategoryId}", responseAsDto.Id);
        return ServiceResult<CreateCategoryResponse>.SuccessAsCreated(responseAsDto, $"api/categories/{responseAsDto.Id}");
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        logger.LogInformation("DeleteAsync called for category ID: {CategoryId}", id);

        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            logger.LogWarning("DeleteAsync failed. Category not found. ID: {CategoryId}", id);
            return ServiceResult.Fail("Kategori bulunamadı.", HttpStatusCode.NotFound);
        }

        categoryRepository.Delete(category);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Category deleted successfully. ID: {CategoryId}", id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CategoryDto>>> GetAllAsync()
    {
        logger.LogInformation("GetAllAsync called.");

        var categories = await categoryRepository.GetAll().ToListAsync();
        var categoriesAsDto = mapper.Map<List<CategoryDto>>(categories);

        logger.LogInformation("GetAllAsync returned {Count} categories.", categoriesAsDto.Count);
        return ServiceResult<List<CategoryDto>>.Success(categoriesAsDto);
    }

    public async Task<ServiceResult<CategoryDto>> GetByIdAsync(int id)
    {
        logger.LogInformation("GetByIdAsync called for category ID: {CategoryId}", id);

        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            logger.LogWarning("GetByIdAsync failed. Category not found. ID: {CategoryId}", id);
            return ServiceResult<CategoryDto>.Fail("Kategori bulunamadı.", HttpStatusCode.NotFound);
        }

        var categoryAsDto = mapper.Map<CategoryDto>(category);

        logger.LogInformation("GetByIdAsync succeeded for category ID: {CategoryId}", id);
        return ServiceResult<CategoryDto>.Success(categoryAsDto);
    }

    public async Task<ServiceResult> UpdateAsync(UpdateCategoryRequest request)
    {
        logger.LogInformation("UpdateAsync called for category ID: {CategoryId}", request.Id);

        var isCategoryNameExist = await categoryRepository
            .Where(x => x.Name == request.Name && x.Id != request.Id)
            .AnyAsync();

        if (isCategoryNameExist)
        {
            logger.LogWarning("UpdateAsync failed. Category name '{CategoryName}' already exists.", request.Name);
            return ServiceResult.Fail("Aynı isimde bir kategori var.", HttpStatusCode.BadRequest);
        }

        var category = await categoryRepository.GetByIdAsync(request.Id);
        if (category is null)
        {
            logger.LogWarning("UpdateAsync failed. Category not found. ID: {CategoryId}", request.Id);
            return ServiceResult.Fail("Kategori bulunamadı.", HttpStatusCode.NotFound);
        }

        mapper.Map(request, category);
        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("UpdateAsync succeeded for category ID: {CategoryId}", request.Id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }


}
