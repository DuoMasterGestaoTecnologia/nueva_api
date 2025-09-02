namespace OmniSuite.Domain.Interfaces
{
    public interface IMfaService
    {
        (string Secret, string QrCodeUri) GenerateMfaSecret(string email);
        string GenerateQrCodeSvg(string qrCodeUri);
        bool ValidateCode(string secret, string code);
    }
}
