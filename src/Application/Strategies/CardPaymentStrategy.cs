using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interface;
using CSharpFunctionalExtensions;

namespace CleanArchitecture.Application.Strategies;
public class CardPaymentStrategy : IPaymentStrategy
{
    public Task<Result> ExecutePaymentAsync(Order order)
    {
        return Task.FromResult(Result.Success());
    }
}
