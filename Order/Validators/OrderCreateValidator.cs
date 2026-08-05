using FluentValidation;
using Order.DTO;

namespace Order.Validators
{
    public class OrderCreateValidator:AbstractValidator<OrderCreateDTO>
    {
        public OrderCreateValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("Invalid Product ID");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero.");
        }
    }
}
