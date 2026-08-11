using FluentValidation;
using ProductService.DTO;

namespace ProductService.Validator
{
    public class ProductValidor : AbstractValidator<ProductCreateDTO>
    {
        public ProductValidor()
        {
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Invalid Product Price");

            RuleFor(x => x.ProductQuantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero.");
        }
    }
}
