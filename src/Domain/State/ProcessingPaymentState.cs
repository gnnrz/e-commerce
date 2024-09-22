using CleanArchitecture.Domain.Interface;
using CSharpFunctionalExtensions;
using Polly;
using Polly.Retry;

namespace CleanArchitecture.Domain.State;
public class ProcessingPaymentState : IOrderState
{
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly int _attempts = 3;

    public ProcessingPaymentState()
    {
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(_attempts, retryAttempt => TimeSpan.FromSeconds(2),
                (exception, timeSpan, retryCount, context) =>
                {
                    // Logando a tentativa de retry
                    Console.WriteLine($"Tentativa {retryCount} falhou: {exception.Message}. Tentando novamente em {timeSpan.TotalSeconds} segundos.");
                });
    }

    public async Task<Result> ProcessAsync(Order order, IPaymentStrategy paymentStrategy, bool usePolly)
    {
        if (usePolly)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var paymentResult = await paymentStrategy.ExecutePaymentAsync(order);

                if (paymentResult.IsSuccess)
                {
                    order.SetState(new PaymentCompletedState());
                    return Result.Success();
                }

                throw new Exception("Falha no processamento do pagamento.");
            });
        }
        else
        {
            var paymentResult = await paymentStrategy.ExecutePaymentAsync(order);

            if (paymentResult.IsSuccess)
            {
                order.SetState(new PaymentCompletedState());
                return Result.Success();
            }

            return Result.Failure("Falha no processamento do pagamento.");
        }
    }

    public Task<Result> CancelAsync(Order order)
    {
        order.SetState(new CanceledState());
        return Task.FromResult(Result.Success());
    }

    public Task<Result> ProcessAsync(Order order)
    {
        return Task.FromResult(Result.Failure("Estratégia de pagamento necessária para processar o pagamento."));
    }
}
