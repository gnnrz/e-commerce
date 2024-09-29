using Castle.Core.Logging;
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
                    Console.WriteLine($"Tentativa {retryCount} falhou: {exception.Message}. Tentando novamente em {timeSpan.TotalSeconds} segundos.");
                });
    }

    public async Task<Result> ProcessAsync(Order order, IPaymentStrategy paymentStrategy, bool usePolly)
    {
        if (usePolly)
        {
            int retryCount = 0;
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                retryCount++;

                var paymentResult = await paymentStrategy.ExecutePaymentAsync(order);

                if (retryCount == 3)
                {
                    order.SetState(new CanceledState());
                    return Result.Failure("Falha no processamento do pagamento após 3 tentativas.");
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
