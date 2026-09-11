using System;
using System.Security.Cryptography;
using System.Text;

namespace PuntoDeVentaWebApi.Helpers
{
    /// <summary>
    /// PBKDF2 (SHA256) para contraseñas nuevas y Rijndael/AES legacy para contraseñas viejas.
    /// </summary>
    public static class Cryptography
    {
        private const int DefaultIterations = 100000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        // Claves legacy (solo para verificar contraseñas almacenadas con el esquema anterior).
        private const string LegacyKey = "PuntoDeVentaDESI2026LegacyKey!!";
        private const string LegacyIv = "PuntoDeVentaIV16";

        public static string HashPassword(string password)
        {
            var salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            var hash = Derive(password, salt, DefaultIterations, HashSize);
            return string.Format("PBKDF2${0}${1}${2}",
                DefaultIterations,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash));
        }

        /// <summary>Verifica formato "PBKDF2$iter$saltB64$hashB64" o fallback legacy.</summary>
        public static bool VerifyPassword(string password, string stored)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(stored))
            {
                return false;
            }

            if (stored.StartsWith("PBKDF2$", StringComparison.Ordinal))
            {
                var parts = stored.Split('$');
                if (parts.Length != 4)
                {
                    return false;
                }

                int iterations;
                if (!int.TryParse(parts[1], out iterations))
                {
                    return false;
                }

                try
                {
                    var salt = Convert.FromBase64String(parts[2]);
                    var expected = Convert.FromBase64String(parts[3]);
                    var actual = Derive(password, salt, iterations, expected.Length);
                    return FixedTimeEquals(expected, actual, expected.Length);
                }
                catch (FormatException)
                {
                    return false;
                }
            }

            return string.Equals(Encrypt(password), stored, StringComparison.Ordinal);
        }

        /// <summary>Verifica PBKDF2 con hash/salt/iteraciones almacenados en columnas separadas.</summary>
        public static bool VerifyPassword(string password, string hashBase64, string saltBase64, int iterations)
        {
            if (string.IsNullOrEmpty(password) ||
                string.IsNullOrWhiteSpace(hashBase64) ||
                string.IsNullOrWhiteSpace(saltBase64) ||
                iterations <= 0)
            {
                return false;
            }

            try
            {
                var salt = Convert.FromBase64String(saltBase64);
                var expected = Convert.FromBase64String(hashBase64);
                var actual = Derive(password, salt, iterations, Math.Max(HashSize, expected.Length));
                return FixedTimeEquals(expected, actual, expected.Length);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static string Encrypt(string plainText)
        {
            if (plainText == null)
            {
                return null;
            }

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(LegacyKey.Substring(0, 32));
                aes.IV = Encoding.UTF8.GetBytes(LegacyIv.Substring(0, 16));
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                var bytes = Encoding.UTF8.GetBytes(plainText);
                var cipher = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
                return Convert.ToBase64String(cipher);
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return cipherText;
            }

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(LegacyKey.Substring(0, 32));
                aes.IV = Encoding.UTF8.GetBytes(LegacyIv.Substring(0, 16));
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                var bytes = Convert.FromBase64String(cipherText);
                var plain = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);
                return Encoding.UTF8.GetString(plain);
            }
        }

        private static byte[] Derive(string password, byte[] salt, int iterations, int size)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(size);
            }
        }

        private static bool FixedTimeEquals(byte[] expected, byte[] actual, int length)
        {
            if (expected == null || actual == null || length <= 0)
            {
                return false;
            }

            var maxLength = Math.Max(expected.Length, actual.Length);
            var diff = expected.Length ^ actual.Length;

            for (var i = 0; i < maxLength; i++)
            {
                var left = i < expected.Length ? expected[i] : (byte)0;
                var right = i < actual.Length ? actual[i] : (byte)0;
                diff |= left ^ right;
            }

            return diff == 0;
        }
    }
}
