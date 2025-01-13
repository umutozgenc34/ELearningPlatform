namespace ELearningPlatform.Model.Baskets.Dtos.Responses;

public record BasketItemDto(
       Guid Id,
       string Name,
       string ImageUrl,
       decimal Price,
       decimal? PriceByApplyDiscountRate);