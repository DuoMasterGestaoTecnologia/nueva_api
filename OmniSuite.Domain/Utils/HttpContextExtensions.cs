using Microsoft.AspNetCore.Http;

namespace OmniSuite.Domain.Utils
{
    public static class HttpContextExtensions
    {
        public static Guid GetUserId(this HttpContext httpContext)
        {
            var userIdClaim = httpContext?.User?.FindFirst("userId");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("Usuário não autenticado ou ID inválido.");

            return userId;
        }
    }
}
