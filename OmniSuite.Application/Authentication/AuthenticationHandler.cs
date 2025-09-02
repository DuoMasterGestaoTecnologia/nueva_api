using OmniSuite.Domain.Utils;

namespace OmniSuite.Application.Authentication
{
    public class AuthenticationHandler :
        IRequestHandler<LoginCommand, Response<AuthenticationResponse>>,
        IRequestHandler<RefreshTokenCommand, Response<AuthenticationResponse>>,
        IRequestHandler<LogoutCommand, Response<bool>>
    {

        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthenticationHandler(ITokenService tokenService, ApplicationDbContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }

        public async Task<Response<AuthenticationResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null)
            {
                return Response<AuthenticationResponse>.Fail("Invalid email or password");
            }

            var accessToken = _tokenService.GenerateToken(user.Id, user.Email, user.Name);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync(cancellationToken);

            var response = new AuthenticationResponse(
                accessToken,
                refreshToken,
                user.Name,
                user.Email,
                DateTime.UtcNow.AddHours(24),
                user.ProfilePhoto
            );

            return Response<AuthenticationResponse>.Ok(response);
        }

        public async Task<Response<AuthenticationResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == request.refreshToken, cancellationToken);

            if (user == null || user.RefreshTokenExpiresAt < DateTime.UtcNow)
                return Response<AuthenticationResponse>.Fail("Refresh token inválido ou expirado.");

            var newAccessToken = _tokenService.GenerateToken(user.Id, user.Email, user.Name);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync(cancellationToken);

            return Response<AuthenticationResponse>.Ok(new AuthenticationResponse(
                newAccessToken,
                newRefreshToken,
                user.Name,
                user.Email,
                DateTime.UtcNow.AddHours(24),
                user.ProfilePhoto
            ));
        }

        public async Task<Response<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = UserClaimsHelper.GetUserId();

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                if (user == null)
                {
                    return Response<bool>.Fail("User not found");
                }

                user.RefreshToken = null;
                user.RefreshTokenExpiresAt = null;

                await _context.SaveChangesAsync(cancellationToken);

                return Response<bool>.Ok(true);
            }
            catch (UnauthorizedAccessException)
            {
                return Response<bool>.Fail("User not authenticated");
            }
        }
    }
}
