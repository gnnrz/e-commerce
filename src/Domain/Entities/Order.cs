using System.Text.Json.Serialization;
using CleanArchitecture.Domain.State;

namespace CleanArchitecture.Domain.Entities;
public class Order
{
    public int Id { get; set; }
    public decimal Total { get; private set; }

    [JsonIgnore]
    public IOrderState State { get; private set; }
    private List<OrderItem> _items = new List<OrderItem>();

    public List<OrderItem> Items
    {
        get => _items;
        set
        {
            _items = value;
            CalculateTotal(); // Garante que o total é calculado quando os itens são definidos
        }
    }

    public Order()
    {
        this.State = new AwaitingProcessingState();
    }

    public void AddItem(OrderItem item)
    {
        _items.Add(item);
        CalculateTotal();
    }

    public void SetState(IOrderState newState)
    {
        State = newState;
    }

    public void CalculateTotal()
    {
        Total = _items.Sum(item => item.Price * item.Quantity);
    }

    public void Process()
    {
        State.ProcessAsync(this);
    }

    public void Cancel()
    {
        State.CancelAsync(this);
    }

    public void ApplyDiscount(decimal discountPercentage)
    {
        Total *= (1 - discountPercentage);
    }

    public string StateDescription => State.GetType().Name;
}
