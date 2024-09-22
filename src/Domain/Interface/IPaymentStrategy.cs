using CSharpFunctionalExtensions;

namespace CleanArchitecture.Domain.Interface;
public interface IPaymentStrategy
{
    Task<Result> ExecutePaymentAsync(Order order);
}
