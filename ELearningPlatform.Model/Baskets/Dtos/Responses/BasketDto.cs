using System.Text.Json.Serialization;

namespace ELearningPlatform.Model.Baskets.Dtos.Responses;

public record BasketDto
{
    [JsonIgnore]
    public string UserId { get; init; }
    public List<BasketItemDto> Items { get; init; } = new();
    public BasketDto(string userId, List<BasketItemDto> items)
    {
        UserId = userId;
        Items = items;
    }
    public BasketDto()
    {

    }
}