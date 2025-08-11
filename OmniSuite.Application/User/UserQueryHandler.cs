using OmniSuite.Application.Generic.Responses;
using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Utils;
using OmniSuite.Infrastructure.Services.MFAService;
using System.Threading;

namespace OmniSuite.Application.User
{
    public class UserQueryHandler :
        IRequestHandler<CreateMFAUserCommand, Response<CreateMFAUserResponse>>,
        IRequestHandler<UserByEmailQuery, UserByEmailResponse>,
        IRequestHandler<UpdatePhotoUserCommand, bool>,
        IRequestHandler<SetupMFAQuery, Response<SetupMFAResponse>>,
        IRequestHandler<UserLoggedQuery, Response<UserLoggedResponse>>,
        IRequestHandler<GetUserQuery, Response<GetUserResponse>>

    {
        private readonly ApplicationDbContext _context;

        public UserQueryHandler(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
        }

        public async Task<UserByEmailResponse> Handle(UserByEmailQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.email, cancellationToken);

            return new UserByEmailResponse(user.Id, user.Name, user.Email);
        }

        public async Task<bool> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            user.Name = command.Name;
            user.Email = command.Email;
            user.PasswordHash = command.PasswordHash;
            user.Status = command.Status;
            user.RefreshToken = command.RefreshToken;
            user.RefreshTokenExpiresAt = command.RefreshTokenExpiresAt;
            user.MfaSecretKey = command.MfaSecretKey;
            user.IsMfaEnabled = command.IsMfaEnabled;
            user.Phone = command.Phone;
            user.Document = command.Document;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> Handle(UpdatePhotoUserCommand request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            using var memoryStream = new MemoryStream();

            await request.DocumentImageBase64.CopyToAsync(memoryStream, cancellationToken);

            var profilePhotoFile = JsonSerializer.Serialize(new Arquivo()
            {
                FileName = request.DocumentImageBase64.FileName,
                ContentType = request.DocumentImageBase64.ContentType,
                Data = Convert.ToBase64String(memoryStream.ToArray())
            });

            user.ProfilePhoto = profilePhotoFile;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<Response<CreateMFAUserResponse>> Handle(CreateMFAUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == UserClaimsHelper.GetUserId(), cancellationToken);

            var isValid = MfaService.ValidateCode(request.Secret, request.Code);

            if (!isValid)
                return Response<CreateMFAUserResponse>.Fail("Código inválido");

            user.MfaSecretKey = request.Secret;
            user.IsMfaEnabled = true;

            var saved = await _context.SaveChangesAsync(cancellationToken);

            var result = new CreateMFAUserResponse(user.Id, true);

            return Response<CreateMFAUserResponse>.Ok(result);
        }
        public async Task<Response<SetupMFAResponse>> Handle(SetupMFAQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == UserClaimsHelper.GetUserId(), cancellationToken);

            var (secret, uri) = MfaService.GenerateMfaSecret(user.Email);
            var qrSvg = MfaService.GenerateQrCodeSvg(uri);

            var response = new SetupMFAResponse
            {
                QrCodeSvg = qrSvg,
                Secret = secret,
            };

            return Response<SetupMFAResponse>.Ok(response);
        }

        public async Task<Response<UserLoggedResponse>> Handle(UserLoggedQuery request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();

            var user = await _context.Users
                .Include(u => u.UserBalance)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            //validate this on validations class
            if (user is null)
            {
                return Response<UserLoggedResponse>.Fail("Nenhum usuario encontrado");
            }


            var response = new UserLoggedResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,      
                Amount = user?.UserBalance?.TotalAmount ?? 0
            };

            return Response<UserLoggedResponse>.Ok(response);
        }

        public async Task<Response<GetUserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();

            var user = await _context.Users
                .Include(u => u.UserBalance)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            //validate this on validations class
            if (user is null)
            {
                return Response<GetUserResponse>.Fail("Nenhum usuario encontrado");
            }

            var response = new GetUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Amount = user?.UserBalance?.TotalAmount ?? 0,
                DocumentNumber = user?.Document,
                CreatedAt = user.CreatedAt,
                Status = user.Status,
                ProfilePhoto = user?.ProfilePhoto
            };

            return Response<GetUserResponse>.Ok(response);
        }
    }

    public class Arquivo
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
}
