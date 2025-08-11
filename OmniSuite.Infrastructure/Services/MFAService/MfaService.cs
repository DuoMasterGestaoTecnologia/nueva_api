using OtpNet;
using QRCoder;

namespace OmniSuite.Infrastructure.Services.MFAService
{
    public static class MfaService
    {
        /// <summary>
        /// Gera um segredo TOTP e a URI OTP para uso com apps como Google Authenticator.
        /// </summary>
        public static (string Secret, string QrCodeUri) GenerateMfaSecret(string email)
        {
            var secret = Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(20));
            var uri = $"otpauth://totp/Flowpag:{email}?secret={secret}&issuer=Flowpag";
            return (secret, uri);
        }

        /// <summary>
        /// Gera um QR Code no formato SVG a partir da URI OTP.
        /// </summary>
        public static string GenerateQrCodeSvg(string qrCodeUri)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
            var svgQr = new SvgQRCode(qrCodeData);
            return svgQr.GetGraphic(5);
        }

        /// <summary>
        /// Valida o código TOTP informado pelo usuário.
        /// </summary>
        public static bool ValidateCode(string secret, string code)
        {
            var totp = new Totp(Base32Encoding.ToBytes(secret));
            return totp.VerifyTotp(code, out _, new VerificationWindow(previous: 1, future: 1));
        }
    }
}
