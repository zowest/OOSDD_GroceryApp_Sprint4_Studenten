using System.Security.Cryptography;
using System.Text;

namespace Grocery.Core.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, 100000, HashAlgorithmName.SHA256, 32);
            return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            // Basic validation
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(storedHash))
                return false;

            // Expect exactly 2 parts: salt.hash
            var parts = storedHash.Split('.', 2);
            if (parts.Length != 2) return false;

            try
            {   
                var salt = Convert.FromBase64String(parts[0]);
                var hash = Convert.FromBase64String(parts[1]);

                // Re-compute hash with same parameters
                var inputHash = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(password),
                    salt,
                    100000,
                    HashAlgorithmName.SHA256,
                    32);

                return CryptographicOperations.FixedTimeEquals(inputHash, hash);
            }
            catch (FormatException)
            {
                // Invalid base64 content
                return false;
            }
        }
    }
}
