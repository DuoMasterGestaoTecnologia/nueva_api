using FluentValidation;

namespace OmniSuite.Application.Account.Validations
{
    public class ForgotPasswordValidation : AbstractValidator<ForgotPasswordCommand>
    {
        private readonly ApplicationDbContext _context;

        public ForgotPasswordValidation(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;

            RuleFor(x => x)
                .MustAsync(ValidateEmailExists)
                .WithMessage("Não foi encontrado nenhum usuário na base de dados com o e-mail informado.");
        }

        private async Task<bool> ValidateEmailExists(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

            return user is not null;
        }
    }
}
