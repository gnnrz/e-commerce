using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace CleanArchitecture.Domain.State;
public class CanceledState : IOrderState
{
    public Task<Result> ProcessAsync(Order order)
    {
        return Task.FromResult(Result.Failure("O pedido está cancelado e não pode ser processado."));
    }

    public Task<Result> CancelAsync(Order order)
    {
        return Task.FromResult(Result.Success());
    }
}
