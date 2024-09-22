using CleanArchitecture.Domain.Interface;
using CSharpFunctionalExtensions;

namespace CleanArchitecture.Domain.State;
public interface IOrderState
{
    Task<Result> ProcessAsync(Order order);
    Task<Result> CancelAsync(Order order);
}
