using Microsoft.AspNetCore.Http;
using System.Text;

namespace OmniSuite.Infrastructure.Services.Security
{
    public abstract class FileSecurity
    {
        private static readonly string[] AllowedMimeTypes = new[]
        {
            "image/png",
            "image/jpeg",
            "image/jpg",
            "image/webp",
            "application/pdf"
        };

        private const int MaxFileSizeInBytes = 10 * 1024 * 1024; // 10MB

        public static async Task<bool> IsSafeAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                return false;

            if (!AllowedMimeTypes.Contains(file.ContentType.ToLower()))
                return false;

            if (file.Length > MaxFileSizeInBytes)
                return false;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();

            return IsValidMagicNumber(fileBytes, file.ContentType);
        }

        private static bool IsValidMagicNumber(byte[] bytes, string contentType)
        {
            if (bytes.Length < 4)
                return false;

            return contentType switch
            {
                "image/png" => bytes.Take(4).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47 }), // PNG
                "image/jpeg" or "image/jpg" => bytes[0] == 0xFF && bytes[1] == 0xD8, // JPEG
                "image/webp" =>
                    Encoding.ASCII.GetString(bytes, 0, 4) == "RIFF" &&
                    Encoding.ASCII.GetString(bytes, 8, 4) == "WEBP", // WEBP
                "application/pdf" => Encoding.ASCII.GetString(bytes, 0, 4) == "%PDF", // PDF
                _ => false
            };
        }
    }
}
