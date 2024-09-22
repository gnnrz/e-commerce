﻿using CleanArchitecture.Application.Validators;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interface;
using CleanArchitecture.Domain.State;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
namespace CleanArchitecture.Application.Service;
public class OrderService
{
    private readonly List<Order> _orders = new List<Order>();
    private readonly ILogger<OrderService> _logger;
    private readonly IValidator<Order> _orderValidator;
    private readonly OrderState _orderState;

    public OrderService(ILogger<OrderService> logger, IValidator<Order> orderValidator, OrderState orderState)
    {
        _logger = logger;
        _orderValidator = orderValidator;
        _orderState = orderState;
    }

    public async Task<Result<Order>> CreateOrderAsync(List<OrderItem> items)
    {
        var order = new Order
        {
            Id = _orderState.GetNextOrderId(),
            Items = items
        };

        var validationResult = await _orderValidator.ValidateAsync(order);
        if (!validationResult.IsValid)
            return Result.Failure<Order>($"Erro na validação do pedido: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");

        order.Process();
        _orderState.AddOrder(order);

        _logger.LogInformation("Pedido {OrderId} realizado com sucesso. Detalhes do pedido: {@Order}", order.Id, order);
        return Result.Success(order);
    }

    public async Task<Result> ProcessPaymentAsync(int orderId, IPaymentStrategy paymentStrategy, bool usePolly)
    {
        var order = await GetOrderByIdAsync(orderId).ContinueWith(t => t.Result.Value);

        if (order == null)
            return Result.Failure("Pedido não encontrado.");

        if (order.State is not ProcessingPaymentState processingPaymentState)
        {
            _logger.LogInformation("Pedido {OrderId} não está em um estado válido para processamento de pagamento. Detalhes do pedido: {@Order}", order.Id, order);
            return Result.Failure("O pedido não está em um estado válido para processamento de pagamento.");
        }

        return await processingPaymentState.ProcessAsync(order, paymentStrategy, usePolly);
    }

    public Task<Maybe<Order>> GetOrderByIdAsync(int id)
    {
        var order = _orderState.GetOrders().FirstOrDefault(o => o.Id == id);
        return Task.FromResult(Maybe.From(order));
    }

    public async Task<Result<IEnumerable<Order>>> GetAllOrdersAsync()
    {
        return await Task.FromResult(Result.Success(_orderState.GetOrders().AsEnumerable()));
    }

    public async Task<Result> CancelOrderAsync(int id)
    {
        var maybeOrder = await GetOrderByIdAsync(id);
        if (maybeOrder.HasNoValue)
            return Result.Failure("Pedido não encontrado.");

        var order = maybeOrder.Value;
        order.Cancel();

        _logger.LogInformation("Pedido {OrderId} foi cancelado com sucesso. Detalhes do pedido: {@Order}", order.Id, order);
        return Result.Success();
    }
}

