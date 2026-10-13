using System.Net;
using Core.Response;
using Core.Services;
using ELearningPlatform.Model.Baskets.Dtos.Responses;
using ELearningPlatform.Model.Discounts.Dtos;
using ELearningPlatform.Model.Order.Entities;
using ELearningPlatform.Model.Order.Enums;
using ELearningPlatform.Model.Payment.Dtos;
using ELearningPlatform.Repository.Orders.Abstracts;
using ELearningPlatform.Service.Baskets.Abstracts;
using ELearningPlatform.Service.Discounts.Abstracts;
using ELearningPlatform.Service.Orders.Abstracts;
using ELearningPlatform.Service.Orders.Concretes;
using ELearningPlatform.Service.Payment.Concretes;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson;
using Moq;

namespace Service.Test;

[TestFixture]
public class OrderServiceTests
{
    private Mock<IOrderRepository> _orderRepositoryMock = null!;
    private Mock<IBasketService> _basketServiceMock = null!;
    private Mock<IIdentityService> _identityMock = null!;
    private Mock<IDiscountService> _discountServiceMock = null!;
    private OrderService _orderService = null!;

    [SetUp]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _basketServiceMock = new Mock<IBasketService>();
        _identityMock = new Mock<IIdentityService>();
        _discountServiceMock = new Mock<IDiscountService>();
        _identityMock.Setup(x => x.GetUserId).Returns("user-1");

        _orderService = new OrderService(_orderRepositoryMock.Object, _basketServiceMock.Object, _identityMock.Object,
            _discountServiceMock.Object, NullLogger<OrderService>.Instance);
    }

    private void GivenBasketWithTotal(decimal price)
    {
        var basket = new BasketDto("user-1", new List<BasketItemDto> { new(Guid.NewGuid(), "Kurs", "", price, null) });
        _basketServiceMock.Setup(x => x.GetBasketAsync()).ReturnsAsync(ServiceResult<BasketDto>.Success(basket));
    }

    [Test]
    public async Task CreateOrder_AppliesDiscount_AndReturnsOrderWithId()
    {
        GivenBasketWithTotal(200m);
        _discountServiceMock.Setup(x => x.GetDiscountByCouponAsync("BABA10"))
            .ReturnsAsync(ServiceResult<DiscountDto>.Success(new DiscountDto("BABA10", 10)));

        var result = await _orderService.CreateOrderAsync("BABA10");

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(result.Data!.TotalPrice, Is.EqualTo(180m));
        Assert.That(result.Data.Status, Is.EqualTo(OrderStatus.Pending));
        Assert.That(result.Data.UserId, Is.EqualTo("user-1"));
        Assert.That(ObjectId.TryParse(result.Data.Id, out _), Is.True);
        _basketServiceMock.Verify(x => x.DeleteBasketAsync(), Times.Once);
    }

    [Test]
    public async Task CreateOrder_WithInvalidCoupon_FailsAndKeepsBasket()
    {
        GivenBasketWithTotal(200m);
        _discountServiceMock.Setup(x => x.GetDiscountByCouponAsync("YANLIS"))
            .ReturnsAsync(ServiceResult<DiscountDto>.Fail("Geçersiz kupon kodu.", HttpStatusCode.NotFound));

        var result = await _orderService.CreateOrderAsync("YANLIS");

        Assert.That(result.IsFail, Is.True);
        _orderRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Order>()), Times.Never);
        _basketServiceMock.Verify(x => x.DeleteBasketAsync(), Times.Never);
    }

    [Test]
    public async Task CreateOrder_WithEmptyBasket_Fails()
    {
        _basketServiceMock.Setup(x => x.GetBasketAsync())
            .ReturnsAsync(ServiceResult<BasketDto>.Success(new BasketDto("user-1", new List<BasketItemDto>())));

        var result = await _orderService.CreateOrderAsync(null);

        Assert.That(result.IsFail, Is.True);
    }

    [Test]
    public async Task GetOrderById_OfAnotherUser_ReturnsNotFound()
    {
        var orderId = ObjectId.GenerateNewId().ToString();
        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync(new Order { Id = orderId, UserId = "someone-else", Status = OrderStatus.Pending });

        var result = await _orderService.GetOrderByIdAsync(orderId);

        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetOrderById_DoesNotChangeStatus()
    {
        var orderId = ObjectId.GenerateNewId().ToString();
        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync(new Order { Id = orderId, UserId = "user-1", Status = OrderStatus.Pending });

        var result = await _orderService.GetOrderByIdAsync(orderId);

        Assert.That(result.Data!.Status, Is.EqualTo(OrderStatus.Pending));
    }
}

[TestFixture]
public class PaymentServiceTests
{
    private Mock<IOrderService> _orderServiceMock = null!;
    private PaymentService _paymentService = null!;
    private readonly string _orderId = ObjectId.GenerateNewId().ToString();

    [SetUp]
    public void Setup()
    {
        _orderServiceMock = new Mock<IOrderService>();
        _paymentService = new PaymentService(_orderServiceMock.Object, NullLogger<PaymentService>.Instance);
    }

    private PaymentRequest Request(string cardNumber = "4111111111111111") => new()
    {
        OrderId = _orderId,
        CardHolderName = "Umut",
        CardNumber = cardNumber,
        ExpiryDate = "12/30",
        Cvv = "123"
    };

    private void GivenOrder(OrderStatus status) =>
        _orderServiceMock.Setup(x => x.GetOrderByIdAsync(_orderId))
            .ReturnsAsync(ServiceResult<Order>.Success(new Order { Id = _orderId, UserId = "user-1", Status = status }));

    [Test]
    public async Task PendingOrder_IsPaid_AndConfirmed()
    {
        GivenOrder(OrderStatus.Pending);

        var result = await _paymentService.ProcessPaymentAsync(Request());

        Assert.That(result.IsSuccess, Is.True);
        _orderServiceMock.Verify(x => x.UpdateOrderStatusAsync(_orderId, OrderStatus.Confirmed), Times.Once);
    }

    [Test]
    public async Task AlreadyConfirmedOrder_CannotBePaidAgain()
    {
        GivenOrder(OrderStatus.Confirmed);

        var result = await _paymentService.ProcessPaymentAsync(Request());

        Assert.That(result.IsFail, Is.True);
        _orderServiceMock.Verify(x => x.UpdateOrderStatusAsync(It.IsAny<string>(), It.IsAny<OrderStatus>()), Times.Never);
    }

    [Test]
    public async Task DeclinedTestCard_LeavesOrderPending()
    {
        GivenOrder(OrderStatus.Pending);

        var result = await _paymentService.ProcessPaymentAsync(Request(PaymentService.DeclinedTestCardNumber));

        Assert.That(result.IsFail, Is.True);
        _orderServiceMock.Verify(x => x.UpdateOrderStatusAsync(It.IsAny<string>(), It.IsAny<OrderStatus>()), Times.Never);
    }
}
