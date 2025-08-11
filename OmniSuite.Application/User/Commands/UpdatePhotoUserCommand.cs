namespace OmniSuite.Application.User.Commands
{
    public class UpdatePhotoUserCommand : IRequest<bool>
    {
        public IFormFile DocumentImageBase64 { get; set; }
    }
}
