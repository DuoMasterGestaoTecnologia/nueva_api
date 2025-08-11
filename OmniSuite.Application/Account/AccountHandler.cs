using System.Security.Cryptography;

namespace OmniSuite.Application.Account
{
    public class AccountHandler :
        IRequestHandler<ForgotPasswordCommand, Response<bool>>,
        IRequestHandler<ResetPasswordCommand, Response<bool>>,
        IRequestHandler<RegisterCommand, Response<CreateUserResponse>>
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<object> _hasher;
        private readonly IEmailService _emailService;

        public AccountHandler(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _hasher = new PasswordHasher<object>();
            _emailService = emailService;
        }

        public async Task<Response<CreateUserResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = request.Name,
                PasswordHash = _hasher.HashPassword(null, request.Password),
                CreatedAt = DateTime.Now,
                IsMfaEnabled = false,
                Phone = UtilsDocument.formatCPFToSaveOnDb(request.Phone),
                Document = UtilsDocument.formatCPFToSaveOnDb(request.Document)
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync(cancellationToken);

            _ = Task.Run(async () =>
            {
                await _emailService.SendWelcomeEmailAsync(request.Email, request.Name);
            });

            return Response<CreateUserResponse>.Ok(new(user.Id, user.Email, user.Name));
        }

        public async Task<Response<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            var userToken = new UserToken
            {
                UserId = user.Id,
                Token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
                Type = UserTokenType.RESET_PASSWORD,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            _context.UserToken.Add(userToken);

            await _context.SaveChangesAsync(cancellationToken);

            _ = Task.Run(async () =>
            {
                await _emailService.SendResetPasswordEmailAsync(
                    user.Email,
                    user.Name,
                    $"https://dashboard.flowpag.com/reset/{userToken.Token}"
                );
            });

            return Response<bool>.Ok(true);
        }

        public async Task<Response<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var token = await _context.UserToken
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == request.token && t.Type == "RESET_PASSWORD", cancellationToken);

            var user = token.User;

            user.PasswordHash = _hasher.HashPassword(null, request.password);

            token.IsUsed = true;
            token.UsedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Response<bool>.Ok(true);
        }
    }
}
