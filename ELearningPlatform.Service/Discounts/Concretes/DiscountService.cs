using AutoMapper;
using Core.Response;
using ELearningPlatform.Model.Discounts.Dtos;
using ELearningPlatform.Model.Discounts.Entity;
using ELearningPlatform.Repository.Discounts.Abstracts;
using ELearningPlatform.Repository.UnitOfWorks.Abstracts;
using ELearningPlatform.Service.Discounts.Abstracts;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ELearningPlatform.Service.Discounts.Concretes;

public class DiscountService(IDiscountRepository discountRepository,IMapper mapper,IUnitOfWork unitOfWork) : IDiscountService
{
    public async Task<ServiceResult<List<DiscountDto>>> GetAllDiscountsAsync()
    {
        var discounts = await discountRepository.GetAll().ToListAsync();
        if (discounts == null || !discounts.Any())
        {
            return ServiceResult<List<DiscountDto>>.Fail("No discounts found.");
        }

        var discountDtos = mapper.Map<List<DiscountDto>>(discounts);
        return ServiceResult<List<DiscountDto>>.Success(discountDtos);
    }

    public async Task<ServiceResult<DiscountDto>> GetDiscountByCouponAsync(string coupon)
    {
        var discount = await discountRepository.Where(d => d.Coupon == coupon).FirstOrDefaultAsync();
        if (discount == null)
        {
            return ServiceResult<DiscountDto>.Fail("Discount with the specified coupon not found.");
        }

        var discountDto = mapper.Map<DiscountDto>(discount);
        return ServiceResult<DiscountDto>.Success(discountDto);
    }

    public async Task<ServiceResult<DiscountDto>> CreateDiscountAsync(DiscountDto request)
    {
        var existingDiscount = await discountRepository.Where(d => d.Coupon == request.Coupon).FirstOrDefaultAsync();
        if (existingDiscount != null)
        {
            return ServiceResult<DiscountDto>.Fail("A discount with the same coupon already exists.");
        }
        var discountEntity = mapper.Map<Discount>(request);

        await discountRepository.AddAsync(discountEntity);
        await unitOfWork.SaveChangesAsync();
       

        var createdDiscountDto = mapper.Map<DiscountDto>(discountEntity);
        return ServiceResult<DiscountDto>.Success(createdDiscountDto, HttpStatusCode.Created);
    }

    public async Task<ServiceResult> DeleteDiscountAsync(string coupon)
    {
        var discount = await discountRepository.Where(d => d.Coupon == coupon).FirstOrDefaultAsync();
        if (discount == null)
        {
            return ServiceResult.Fail("Discount with the specified coupon not found.");
        }

        discountRepository.Delete(discount);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success("Discount deleted successfully.");
    }
}
