using System;
using System.Security.Cryptography;

namespace rentalsystem
{
    public static class PasswordHelper
    {
        // Format: {iterations}.{salt(Base64)}.{hash(Base64)}
        public static string HashPassword(string password, int iterations = 10000)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var salt = new byte[16];
                rng.GetBytes(salt);
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
                {
                    var hash = pbkdf2.GetBytes(32);
                    return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
                }
            }
        }

        public static bool VerifyPassword(string stored, string password)
        {
            if (string.IsNullOrEmpty(stored) || string.IsNullOrEmpty(password)) return false;
            var parts = stored.Split('.');
            if (parts.Length != 3) return false;
            if (!int.TryParse(parts[0], out var iterations)) return false;
            var salt = Convert.FromBase64String(parts[1]);
            var storedHash = Convert.FromBase64String(parts[2]);
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                var hash = pbkdf2.GetBytes(storedHash.Length);
                // constant time comparison
                var diff = 0;
                for (int i = 0; i < storedHash.Length; i++) diff |= storedHash[i] ^ hash[i];
                return diff == 0;
            }
        }
    }
}
