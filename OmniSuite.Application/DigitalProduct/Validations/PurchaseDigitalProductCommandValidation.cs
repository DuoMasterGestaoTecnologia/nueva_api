using FluentValidation;

namespace OmniSuite.Application.DigitalProduct.Validations
{
    public class PurchaseDigitalProductCommandValidation : AbstractValidator<Commands.PurchaseDigitalProductCommand>
    {
        public PurchaseDigitalProductCommandValidation()
        {
            RuleFor(x => x.DigitalProductId)
                .NotEmpty().WithMessage("ID do produto digital é obrigatório");
        }
    }
}
