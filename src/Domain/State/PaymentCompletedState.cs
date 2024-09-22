using CSharpFunctionalExtensions;

namespace CleanArchitecture.Domain.State;
public class PaymentCompletedState : IOrderState
{
    public Task<Result> ProcessAsync(Order order)
    {
        order.SetState(new SeparatingOrderState());

        return Task.FromResult(Result.Success());
    }

    public Task<Result> CancelAsync(Order order)
    {
        order.SetState(new CanceledState());

        return Task.FromResult(Result.Success());
    }
}
