using System;
using System.Configuration;
using PuntoDeVenta.WebApi.Dal;

namespace PuntoDeVenta.WebApi.Services.Auth
{
    internal class AuthService
    {
        private readonly AuthDal _authDal;
        private readonly Pbkdf2PasswordVerifier _passwordVerifier;

        public AuthService()
        {
            _authDal = new AuthDal();
            _passwordVerifier = new Pbkdf2PasswordVerifier();
        }

        public bool IsValidClient(string clientId, string clientSecret)
        {
            var expectedClientId = ConfigurationManager.AppSettings["client_id"];
            var expectedClientSecret = ConfigurationManager.AppSettings["client_secret"];

            if (string.IsNullOrWhiteSpace(expectedClientId) || string.IsNullOrWhiteSpace(expectedClientSecret))
            {
                return false;
            }

            var normalizedClientId = clientId?.Trim();
            var normalizedClientSecret = clientSecret?.Trim();

            return string.Equals(expectedClientId, normalizedClientId, StringComparison.Ordinal) &&
                   string.Equals(expectedClientSecret, normalizedClientSecret, StringComparison.Ordinal);
        }

        public AuthUserRecord Authenticate(string userName, string password)
        {
            var user = _authDal.ObtenerUsuarioParaLogin(userName);
            if (user == null || !user.Estatus)
            {
                return null;
            }

            var passwordOk = _passwordVerifier.Verify(password, user.ContrasenaSalt, user.ContrasenaIteraciones, user.ContrasenaHash);
            if (!passwordOk)
            {
                return null;
            }

            return user;
        }
    }
}
