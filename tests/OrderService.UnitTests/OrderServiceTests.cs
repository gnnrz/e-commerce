using CleanArchitecture.Application.Service;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interface;
using CleanArchitecture.Domain.State;
using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
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

        // Criando um mock para IValidator<Order>
        _orderValidatorMock = new Mock<IValidator<Order>>();

        // Configurando o mock para retornar sucesso sempre que o método ValidateAsync for chamado
        _orderValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<Order>(), default))
            .ReturnsAsync(new ValidationResult());

        // Criando um mock para OrderState
        var orderStateMock = new Mock<OrderState>();

        // Passando o logger, o validator mock e o OrderState mock para o OrderService
        _orderService = new OrderService(loggerMock.Object, _orderValidatorMock.Object, orderStateMock.Object);
    }

    [Fact]
    public async Task CreateOrderAsync_Should_Create_Order_Successfully()
    {
        // Arrange
        var items = new List<OrderItem>
        {
            new OrderItem("Produto A", 10.00m, 2),
            new OrderItem("Produto B", 5.00m, 1)
        };

        // Act
        var result = await _orderService.CreateOrderAsync(items);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Items.Count);
        Assert.Equal(25.00m, result.Value.Total);
    }

    [Fact]
    public async Task ProcessPaymentAsync_Should_Return_Failure_When_Payment_Fails()
    {
        // Arrange
        var order = new Order();
        order.AddItem(new OrderItem("Produto A", 10.00m, 2));

        var mockPaymentStrategy = new Mock<IPaymentStrategy>();
        mockPaymentStrategy.Setup(p => p.ExecutePaymentAsync(It.IsAny<Order>())).ReturnsAsync(Result.Failure("Falha no pagamento"));

        await _orderService.CreateOrderAsync(order.Items); // Adicionando o pedido

        // Act
        var result = await _orderService.ProcessPaymentAsync(order.Id, mockPaymentStrategy.Object, false);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Falha no pagamento", result.Error);
    }
}
