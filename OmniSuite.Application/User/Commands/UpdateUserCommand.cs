using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniSuite.Application.User.Commands
{
    public class UpdateUserCommand : IRequest<Response<bool>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserStatusEnum Status { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
        public string? MfaSecretKey { get; set; }
        public bool? IsMfaEnabled { get; set; }
        public string? Phone { get; set; }
        public string? Document { get; set; }
    }
}
