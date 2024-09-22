using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace CleanArchitecture.Domain.State;
public class AwaitingStockState : IOrderState
{
    public async Task<Result> ProcessAsync(Order order)
    {
        return await Task.FromResult(Result.Success("O pedido está aguardando reposição de estoque."));
    }

    public async Task<Result> CancelAsync(Order order)
    {
        order.SetState(new CanceledState());

        // Retorne o resultado de forma assíncrona
        return await Task.FromResult(Result.Success());
    }
}
