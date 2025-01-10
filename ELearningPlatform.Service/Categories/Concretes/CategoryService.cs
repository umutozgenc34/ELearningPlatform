using AutoMapper;
using Core.Response;
using ELearningPlatform.Model.Categories.Dtos.Request;
using ELearningPlatform.Model.Categories.Dtos.Response;
using ELearningPlatform.Model.Categories.Entity;
using ELearningPlatform.Repository.Categories.Abstracts;
using ELearningPlatform.Repository.UnitOfWorks.Abstracts;
using ELearningPlatform.Service.Categories.Abstracts;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ELearningPlatform.Service.Categories.Concretes;

public class CategoryService(ICategoryRepository categoryRepository,IMapper mapper,IUnitOfWork unitOfWork) : ICategoryService
{
    public async Task<ServiceResult<CreateCategoryResponse>> CreateAsync(CreateCategoryRequest request)
    {
        var hasCategory = await categoryRepository.Where(x=> x.Name == request.Name).AnyAsync();
        if (hasCategory is true)
        {
            return ServiceResult<CreateCategoryResponse>.Fail("Aynı isimde bir kategori var.",HttpStatusCode.BadRequest);
        }

        var category = mapper.Map<Category>(request);
        await categoryRepository.AddAsync(category);
        await unitOfWork.SaveChangesAsync();

        var responseAsDto = mapper.Map<CreateCategoryResponse>(category);

        return ServiceResult<CreateCategoryResponse>.SuccessAsCreated(responseAsDto, $"api/categories/{responseAsDto.Id}");
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var category =await categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            return ServiceResult.Fail("Kategori bulunamadı.",HttpStatusCode.NotFound);
        }

        categoryRepository.Delete(category);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CategoryDto>>> GetAllAsync()
    {
        var categories = await categoryRepository.GetAll().ToListAsync();

        var categoriesAsDto = mapper.Map<List<CategoryDto>>(categories);

        return ServiceResult<List<CategoryDto>>.Success(categoriesAsDto);
    }

    public async Task<ServiceResult<CategoryDto>> GetByIdAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            return ServiceResult<CategoryDto>.Fail("Kategori bulunamadı.",HttpStatusCode.NotFound);
        }

        var categoryAsDto = mapper.Map<CategoryDto>(category);

        return ServiceResult<CategoryDto>.Success(categoryAsDto);
    }

    public async Task<ServiceResult> UpdateAsync(UpdateCategoryRequest request)
    {
        var isCategoryNameExist = await categoryRepository.Where(x => x.Name == request.Name && x.Id != request.Id).AnyAsync();

        if (isCategoryNameExist is true)
        {
            return ServiceResult.Fail("Aynı isimde bir kategori var.",HttpStatusCode.BadRequest);
        }

        var category = await categoryRepository.GetByIdAsync(request.Id);
        if (category is null)
        {
            return ServiceResult.Fail("Kategori bulunamadı.",HttpStatusCode.NotFound);
        }

        var categoryAsDto = mapper.Map(request,category);

        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
