using System.Security.Cryptography;
using System.Text;

namespace Lab04.Security
{
    public static class RefreshTokenHasher
    {
        public static string Hash(string rawToken)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(rawToken)));
        }

        public static string GenerateRawToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }
    }
}
