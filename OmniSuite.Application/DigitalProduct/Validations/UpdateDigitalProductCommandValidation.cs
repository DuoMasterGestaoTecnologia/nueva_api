using FluentValidation;

namespace OmniSuite.Application.DigitalProduct.Validations
{
    public class UpdateDigitalProductCommandValidation : AbstractValidator<Commands.UpdateDigitalProductCommand>
    {
        public UpdateDigitalProductCommandValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID é obrigatório");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Descrição é obrigatória")
                .MaximumLength(2000).WithMessage("Descrição deve ter no máximo 2000 caracteres");

            RuleFor(x => x.ShortDescription)
                .MaximumLength(500).WithMessage("Descrição curta deve ter no máximo 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.ShortDescription));

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Preço deve ser maior que zero");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Tipo de produto inválido");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Status inválido");

            RuleFor(x => x.DownloadLimit)
                .GreaterThan(0).WithMessage("Limite de downloads deve ser maior que zero")
                .When(x => x.DownloadLimit.HasValue);

            RuleFor(x => x.ExpirationDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Data de expiração deve ser futura")
                .When(x => x.ExpirationDate.HasValue);

            RuleFor(x => x.Category)
                .MaximumLength(100).WithMessage("Categoria deve ter no máximo 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Category));

            RuleFor(x => x.Tags)
                .MaximumLength(500).WithMessage("Tags devem ter no máximo 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Tags));
        }
    }
}
