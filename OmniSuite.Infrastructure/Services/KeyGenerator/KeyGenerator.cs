using System.Security.Cryptography;

namespace OmniSuite.Infrastructure.Services.KeyGenerator
{
    public static class KeyGenerator
    {
        public static string GeneratePublicKey()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        public static string GeneratePrivateKey()
        {
            var key = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(key);
        }

        public static string GenerateHexKey(int bytes = 32)
        {
            var buffer = RandomNumberGenerator.GetBytes(bytes);
            return BitConverter.ToString(buffer).Replace("-", "").ToLower();
        }
    }

}
