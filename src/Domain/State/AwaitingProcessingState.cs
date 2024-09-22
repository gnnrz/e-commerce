using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace CleanArchitecture.Domain.State;
public class AwaitingProcessingState : IOrderState
{
    public async Task<Result> ProcessAsync(Order order)
    {
        order.SetState(new ProcessingPaymentState());

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> CancelAsync(Order order)
    {
        order.SetState(new CanceledState());

        return await Task.FromResult(Result.Success());
    }
}

