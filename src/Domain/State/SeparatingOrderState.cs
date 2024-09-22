using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace CleanArchitecture.Domain.State;
public class SeparatingOrderState : IOrderState
{
    public async Task<Result> ProcessAsync(Order order)
    {
        bool isStockAvailable = CheckStock(order);

        if (isStockAvailable)
        {
            DeductStock(order);

            order.SetState(new CompletedState());

            return await Task.FromResult(Result.Success());
        }
        else
        {
            order.SetState(new AwaitingStockState());

            NotifySalesTeam(order);

            return await Task.FromResult(Result.Failure("Estoque insuficiente. Pedido movido para o estado de 'Aguardando Estoque'."));
        }
    }

    public Task<Result> CancelAsync(Order order)
    {
        order.SetState(new CanceledState());

        return Task.FromResult(Result.Success());
    }

    private bool CheckStock(Order order)
    {
        return order.Items.All(item => item.Quantity <= GetAvailableStock(item.ProductName));
    }

    private void DeductStock(Order order)
    {
    }

    private int GetAvailableStock(string productName)
    {
        return 10;
    }

    private void DeductItemStock(string productName, int quantity)
    {
    }

    private void NotifySalesTeam(Order order)
    {
    }
}
