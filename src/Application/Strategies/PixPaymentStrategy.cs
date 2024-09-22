using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interface;
using CSharpFunctionalExtensions;

namespace CleanArchitecture.Application.Strategies;
public class PixPaymentStrategy : IPaymentStrategy
{
    public Task<Result> ExecutePaymentAsync(Order order)
    {
        order.ApplyDiscount(0.05m);

        return Task.FromResult(Result.Success());
    }
}
