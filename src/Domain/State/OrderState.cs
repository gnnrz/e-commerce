using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.State;
public class OrderState
{
    private readonly List<Order> _orders = new List<Order>();
    private int _nextOrderId = 1;

    public List<Order> GetOrders() => _orders;

    public int GetNextOrderId() => _nextOrderId++;

    public void AddOrder(Order order) => _orders.Add(order);
}
