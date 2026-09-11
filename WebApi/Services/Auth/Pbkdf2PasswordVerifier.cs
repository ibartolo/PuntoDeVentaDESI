using System;
using System.Security.Cryptography;
using System.Text;

namespace PuntoDeVenta.WebApi.Services.Auth
{
    internal class Pbkdf2PasswordVerifier
    {
        private const int MinIterations = 100000;
        private const int HashSize = 32;

        public bool Verify(string plainPassword, string saltBase64, int iterations, string expectedHashBase64)
        {
            if (string.IsNullOrEmpty(plainPassword) ||
                string.IsNullOrWhiteSpace(saltBase64) ||
                string.IsNullOrWhiteSpace(expectedHashBase64) ||
                iterations < MinIterations)
            {
                return false;
            }

            byte[] salt;
            byte[] expectedHash;

            try
            {
                salt = Convert.FromBase64String(saltBase64);
                expectedHash = Convert.FromBase64String(expectedHashBase64);
            }
            catch (FormatException)
            {
                return false;
            }

            if (salt.Length == 0 || expectedHash.Length == 0)
            {
                return false;
            }

            byte[] computedHash;
            using (var deriveBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(plainPassword), salt, iterations, HashAlgorithmName.SHA256))
            {
                computedHash = deriveBytes.GetBytes(Math.Max(HashSize, expectedHash.Length));
            }

            return ConstantTimeEquals(expectedHash, computedHash, expectedHash.Length);
        }

        private static bool ConstantTimeEquals(byte[] expected, byte[] actual, int length)
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
