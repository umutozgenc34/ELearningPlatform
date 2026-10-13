using System.Net;
using System.Security.Claims;
using Core.Services;
using ELearningPlatform.Model.Baskets.Dtos.Request;
using ELearningPlatform.Model.Courses.Dtos.Response;
using Core.Response;
using ELearningPlatform.Service.Baskets.Concretes;
using ELearningPlatform.Service.Courses.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace Service.Test;

[TestFixture]
public class IdentityServiceTests
{
    [Test]
    public void GetUserId_ReadsNameIdentifierClaimFromToken()
    {
        // TokenService kullanıcı Id'sini ClaimTypes.NameIdentifier olarak yazıyor.
        // Eskiden "user_id" claim'i arandığı için Id hep boş geliyordu.
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-123"),
                new Claim(ClaimTypes.Name, "umut"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "test"))
        };
        var accessor = new HttpContextAccessor { HttpContext = httpContext };

        var identityService = new IdentityService(accessor);

        Assert.That(identityService.GetUserId, Is.EqualTo("user-123"));
        Assert.That(identityService.UserName, Is.EqualTo("umut"));
        Assert.That(identityService.IsInRole("Admin"), Is.True);
    }

    [Test]
    public void GetUserId_WithoutHttpContext_ReturnsEmpty()
    {
        var identityService = new IdentityService(new HttpContextAccessor());
        Assert.That(identityService.GetUserId, Is.Empty);
    }
}

[TestFixture]
public class BasketServiceTests
{
    private IDistributedCache _cache = null!;
    private Mock<ICourseService> _courseServiceMock = null!;
    private Mock<IIdentityService> _identityMock = null!;
    private BasketService _basketService = null!;

    [SetUp]
    public void Setup()
    {
        _cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        _courseServiceMock = new Mock<ICourseService>();
        _identityMock = new Mock<IIdentityService>();
        _basketService = new BasketService(_cache, _courseServiceMock.Object, _identityMock.Object, NullLogger<BasketService>.Instance);
    }

    [Test]
    public async Task GetBasket_WhenBasketDoesNotExist_ReturnsEmptyBasket()
    {
        _identityMock.Setup(x => x.GetUserId).Returns("user-1");

        var result = await _basketService.GetBasketAsync();

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data!.Items, Is.Empty);
        Assert.That(result.Data.TotalPrice, Is.EqualTo(0));
    }

    [Test]
    public async Task AddBasketItem_WithoutUserId_IsRejected()
    {
        _identityMock.Setup(x => x.GetUserId).Returns(string.Empty);

        var result = await _basketService.AddBasketItemAsync(new CreateBasketItemRequest(Guid.NewGuid()));

        Assert.That(result.IsFail, Is.True);
        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Baskets_AreSeparatedPerUser()
    {
        var courseId = Guid.NewGuid();
        _courseServiceMock.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync(
            ServiceResult<CourseDto>.Success(new CourseDto { Id = courseId, Name = "C#", Price = 100m, ImageUrl = "" }));

        _identityMock.Setup(x => x.GetUserId).Returns("user-1");
        await _basketService.AddBasketItemAsync(new CreateBasketItemRequest(courseId));
        await _basketService.AddBasketItemAsync(new CreateBasketItemRequest(courseId)); // aynı kurs ikinci kez eklenmez

        var user1Basket = await _basketService.GetBasketAsync();

        _identityMock.Setup(x => x.GetUserId).Returns("user-2");
        var user2Basket = await _basketService.GetBasketAsync();

        Assert.That(user1Basket.Data!.Items, Has.Count.EqualTo(1));
        Assert.That(user1Basket.Data.TotalPrice, Is.EqualTo(100m));
        Assert.That(user2Basket.Data!.Items, Is.Empty);
    }
}
