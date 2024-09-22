using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Validators;
public class OrderValidator : AbstractValidator<Order>
{
    public OrderValidator()
    {
        RuleFor(order => order.Items)
            .NotEmpty().WithMessage("O pedido deve conter pelo menos um item")
            .Must(items => items.All(item => item.Quantity > 0))
            .WithMessage("Todos os itens do pedido devem ter quantidade maior que zero");

        RuleForEach(order => order.Items).SetValidator(new OrderItemValidator());
    }
}
