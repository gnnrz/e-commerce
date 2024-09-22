using CSharpFunctionalExtensions;

namespace CleanArchitecture.Domain.State;
public class CompletedState : IOrderState
{
    public Task<Result> ProcessAsync(Order order)
    {
        return Task.FromResult(Result.Failure("O pedido já foi concluído e não pode ser processado novamente."));
    }

    public Task<Result> CancelAsync(Order order)
    {
        return Task.FromResult(Result.Failure("Não é possível cancelar um pedido que já foi concluído."));
    }
}
