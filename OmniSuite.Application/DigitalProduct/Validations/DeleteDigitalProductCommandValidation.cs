using FluentValidation;

namespace OmniSuite.Application.DigitalProduct.Validations
{
    public class DeleteDigitalProductCommandValidation : AbstractValidator<Commands.DeleteDigitalProductCommand>
    {
        public DeleteDigitalProductCommandValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID é obrigatório");
        }
    }
}
