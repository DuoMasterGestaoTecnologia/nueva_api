using FluentValidation;

namespace OmniSuite.Application.Account.Validations
{
    public class ResetPasswordValidation : AbstractValidator<ResetPasswordCommand>
    {
        private readonly ApplicationDbContext _context;

        public ResetPasswordValidation(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.token)
                .NotEmpty().WithMessage("Token é obrigatório.")
                .MustAsync(BeAValidToken).WithMessage("Token inválido, expirado ou já utilizado.");

            RuleFor(x => x.password)
                .NotEmpty().WithMessage("Senha é obrigatória.")
                .MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres.");
        }

        private async Task<bool> BeAValidToken(string tokenValue, CancellationToken cancellationToken)
        {
            var token = await _context.UserToken
                .Include(t => t.User)
                .FirstOrDefaultAsync(t =>
                    t.Token == tokenValue &&
                    t.Type == "RESET_PASSWORD", cancellationToken);

            if (token == null || token.IsUsed || token.ExpiresAt < DateTime.UtcNow || token.User == null)
                return false;

            return true;
        }
    }
}
