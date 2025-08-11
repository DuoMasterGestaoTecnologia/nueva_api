using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace OmniSuite.Domain.Utils
{
    public static class UserClaimsHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private static ClaimsPrincipal? User => _httpContextAccessor?.HttpContext?.User;

        public static Guid GetUserId()
        {
            var claim = User?.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(claim) || !Guid.TryParse(claim, out var userId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            return userId;
        }

        public static string? GetEmail()
        {
            return User?.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static string? GetRole()
        {
            return User?.FindFirst(ClaimTypes.Role)?.Value;
        }

        public static string? GetClaim(string claimType)
        {
            return User?.FindFirst(claimType)?.Value;
        }
    }
}
