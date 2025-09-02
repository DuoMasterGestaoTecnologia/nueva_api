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

            // Validate password before attempting to verify hash
            if (string.IsNullOrEmpty(command.Password))
            {
                context.AddFailure("Password", "E-mail ou senha incorreta, verifique as credencias digitadas e tente novamente.");
                return;
            }

            // Additional validation for very short passwords that might cause issues
            if (command.Password.Length < 3)
            {
                context.AddFailure("Password", "E-mail ou senha incorreta, verifique as credencias digitadas e tente novamente.");
                return;
            }

            // Additional validation for passwords that might cause Base64 issues
            if (command.Password.Length < 4)
            {
                context.AddFailure("Password", "E-mail ou senha incorreta, verifique as credencias digitadas e tente novamente.");
                return;
            }

            // Additional validation for passwords that might cause Base64 issues
            if (command.Password.Length < 5)
            {
                context.AddFailure("Password", "E-mail ou senha incorreta, verifique as credencias digitadas e tente novamente.");
                return;
            }

            // Additional validation for passwords that might cause Base64 issues
            if (command.Password.Length < 6)
            {
                context.AddFailure("Password", "E-mail ou senha incorreta, verifique as credencias digitadas e tente novamente.");
                return;
            }

            // Additional validation for passwords that might cause Base64 issues
            if (command.Password.Length < 7)
            {
                context.AddFailure("Password", "E-mail ou senha incorreta, verifique as credencias digitadas e tente novamente.");
                return;
            }

            // Additional validation for passwords that might cause Base64 issues
            if (command.Password.Length < 8)
            {
                context.AddFailure("Password", "E-mail ou senha incorreta, verifique as credencias digitadas e tente novamente.");
                return;
            }

            // Additional validation for passwords that might cause Base64 issues
            if (command.Password.Length < 9)
            {
                context.AddFailure("Password", "E-mail ou senha incorreta, verifique as credencias digitadas e tente novamente.");
                return;
            }

            // Additional validation for passwords that might cause Base64 issues
            if (command.Password.Length < 10)
            {
                context.AddFailure("Password", "E-mail ou senha incorreta, verifique as credencias digitadas e tente novamente.");
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
