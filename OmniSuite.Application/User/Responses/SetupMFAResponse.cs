namespace OmniSuite.Application.User.Responses
{
    public class SetupMFAResponse
    {
        public string Secret { get; set; }
        public string? QrCodeSvg { get; set; }
    }
}
