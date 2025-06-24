using AutoMapper;
using Core.Response;
using ELearningPlatform.Model.Discounts.Dtos;
using ELearningPlatform.Model.Discounts.Entity;
using ELearningPlatform.Repository.Discounts.Abstracts;
using ELearningPlatform.Repository.UnitOfWorks.Abstracts;
using ELearningPlatform.Service.Discounts.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ELearningPlatform.Service.Discounts.Concretes;

public class DiscountService(IDiscountRepository discountRepository,IMapper mapper,IUnitOfWork unitOfWork, ILogger<DiscountService> logger) : IDiscountService
{
    public async Task<ServiceResult<List<DiscountDto>>> GetAllDiscountsAsync()
    {
        logger.LogInformation("GetAllDiscountsAsync called.");

        var discounts = await discountRepository.GetAll().ToListAsync();
        if (discounts == null || !discounts.Any())
        {
            logger.LogWarning("No discounts found.");
            return ServiceResult<List<DiscountDto>>.Fail("No discounts found.");
        }

        var discountDtos = mapper.Map<List<DiscountDto>>(discounts);
        logger.LogInformation("GetAllDiscountsAsync returned {Count} discounts.", discountDtos.Count);
        return ServiceResult<List<DiscountDto>>.Success(discountDtos);
    }

    public async Task<ServiceResult<DiscountDto>> GetDiscountByCouponAsync(string coupon)
    {
        logger.LogInformation("GetDiscountByCouponAsync called for coupon: {Coupon}", coupon);

        var discount = await discountRepository.Where(d => d.Coupon == coupon).FirstOrDefaultAsync();
        if (discount == null)
        {
            logger.LogWarning("Discount not found for coupon: {Coupon}", coupon);
            return ServiceResult<DiscountDto>.Fail("Discount with the specified coupon not found.");
        }

        var discountDto = mapper.Map<DiscountDto>(discount);
        logger.LogInformation("Discount found for coupon: {Coupon}", coupon);
        return ServiceResult<DiscountDto>.Success(discountDto);
    }

    public async Task<ServiceResult<DiscountDto>> CreateDiscountAsync(DiscountDto request)
    {
        logger.LogInformation("CreateDiscountAsync called for coupon: {Coupon}", request.Coupon);

        var existingDiscount = await discountRepository
            .Where(d => d.Coupon == request.Coupon)
            .FirstOrDefaultAsync();

        if (existingDiscount != null)
        {
            logger.LogWarning("CreateDiscountAsync failed. Discount with coupon '{Coupon}' already exists.", request.Coupon);
            return ServiceResult<DiscountDto>.Fail("A discount with the same coupon already exists.");
        }

        var discountEntity = mapper.Map<Discount>(request);

        await discountRepository.AddAsync(discountEntity);
        await unitOfWork.SaveChangesAsync();

        var createdDiscountDto = mapper.Map<DiscountDto>(discountEntity);
        logger.LogInformation("Discount created successfully. Coupon: {Coupon}", createdDiscountDto.Coupon);
        return ServiceResult<DiscountDto>.Success(createdDiscountDto, HttpStatusCode.Created);
    }

    public async Task<ServiceResult> DeleteDiscountAsync(string coupon)
    {
        logger.LogInformation("DeleteDiscountAsync called for coupon: {Coupon}", coupon);

        var discount = await discountRepository.Where(d => d.Coupon == coupon).FirstOrDefaultAsync();
        if (discount == null)
        {
            logger.LogWarning("DeleteDiscountAsync failed. Discount not found for coupon: {Coupon}", coupon);
            return ServiceResult.Fail("Discount with the specified coupon not found.");
        }

        discountRepository.Delete(discount);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Discount deleted successfully. Coupon: {Coupon}", coupon);
        return ServiceResult.Success("Discount deleted successfully.");
    }
}
