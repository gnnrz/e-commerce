namespace CleanArchitecture.Web.DTOs;

public class PaymentResultDto
{
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemDto>? Items { get; set; }
}

public class OrderItemDto
{
    public string? ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

}
