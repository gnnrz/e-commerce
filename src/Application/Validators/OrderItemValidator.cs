using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Validators;
public class OrderItemValidator : AbstractValidator<OrderItem>
{
    public OrderItemValidator()
    {
        RuleFor(item => item.ProductName)
            .NotEmpty().WithMessage("O nome do produto não pode estar vazio");

        RuleFor(item => item.Price)
            .GreaterThan(0).WithMessage("O preço do produto deve ser maior que zero");

    }
}
