using CleanArchitecture.Application.Service;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interface;
using CleanArchitecture.Domain.State;
using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class OrderServiceTests
{
    private readonly OrderService _orderService;
    private readonly Mock<IValidator<Order>> _orderValidatorMock;

    public OrderServiceTests()
    {
        var loggerMock = new Mock<ILogger<OrderService>>();

        _orderValidatorMock = new Mock<IValidator<Order>>();

        _orderValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<Order>(), default))
            .ReturnsAsync(new ValidationResult());

        var orderStateMock = new Mock<OrderState>();

        var memoryCacheMock = new Mock<IMemoryCache>();

        _orderService = new OrderService(loggerMock.Object, _orderValidatorMock.Object, orderStateMock.Object, memoryCacheMock.Object);
    }

    [Fact]
    public async Task CreateOrderAsync_Should_Create_Order_Successfully()
    {
        var items = new List<OrderItem>
        {
            new OrderItem("Produto A", 10.00m, 2),
            new OrderItem("Produto B", 5.00m, 1)
        };

        var result = await _orderService.CreateOrderAsync(items);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Items.Count);
        Assert.Equal(25.00m, result.Value.Total);
    }

    [Fact]
    public async Task ProcessPaymentAsync_Should_Return_Failure_When_Payment_Fails()
    {
        var order = new Order();
        order.AddItem(new OrderItem("Produto A", 10.00m, 2));

        var mockPaymentStrategy = new Mock<IPaymentStrategy>();
        mockPaymentStrategy.Setup(p => p.ExecutePaymentAsync(It.IsAny<Order>())).ReturnsAsync(Result.Failure("Falha no pagamento"));

        await _orderService.CreateOrderAsync(order.Items); 

        var result = await _orderService.ProcessPaymentAsync(order.Id, mockPaymentStrategy.Object, false);

        Assert.True(result.IsFailure);
        Assert.Equal("Falha no pagamento", result.Error);
    }
}
