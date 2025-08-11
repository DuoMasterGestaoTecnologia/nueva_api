using FluentValidation;
using OmniSuite.Application.Common.Utils;

namespace OmniSuite.Application.Account.Validations
{
    public class RegisterValidation : AbstractValidator<RegisterCommand>
    {
        private readonly ApplicationDbContext _context;

        public RegisterValidation(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;

            RuleFor(x => x)
                .MustAsync(ValidUserExists)
                .WithMessage("Já existe uma conta cadastrada com este e-mail.");         
        }

        private async Task<bool> ValidUserExists(RegisterCommand command, CancellationToken cancellationToken)
        {
            var formattedDocument = UtilsDocument.formatCPFToSaveOnDb(command.Document);

            var users = await _context.Users
                .Where(x => x.Email == command.Email)
                .ToListAsync(cancellationToken);

            return !users.Any();
        }        
    }
}
