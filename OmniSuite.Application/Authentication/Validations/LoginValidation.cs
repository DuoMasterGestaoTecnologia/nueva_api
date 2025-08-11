using FluentValidation;

namespace OmniSuite.Application.Authentication.Validations
{
    public class LoginValidation : AbstractValidator<LoginCommand>
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<object> _hasher;

        public LoginValidation(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _hasher = new PasswordHasher<object>();

            RuleFor(x => x)
                .CustomAsync(ValidateUser);
        }

        private async Task ValidateUser(LoginCommand command, ValidationContext<LoginCommand> context, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

            if (user is null)
            {
                context.AddFailure("User", "Usuário não encontrado na base de dados");
                return;
            }

            if (user.Status == UserStatusEnum.inactive)
            {
                context.AddFailure("Account", "Usuário está desativado, entre em contato com o suporte para mais informações.");
                return;
            }

            var result = _hasher.VerifyHashedPassword(null, user.PasswordHash, command.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                context.AddFailure("Password", "E-mail ou senha incorreta, verifique as credencias digitadas e tente novamente.");
                return;
            }
        }
    }
}
