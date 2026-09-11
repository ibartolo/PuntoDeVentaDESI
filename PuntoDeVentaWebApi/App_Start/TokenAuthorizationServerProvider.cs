using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace PuntoDeVentaWebApi.App_Start
{
    public class TokenAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            context.TryGetFormCredentials(out clientId, out clientSecret);

            var expectedClientId = ConfigurationManager.AppSettings["client_id"];
            var expectedClientSecret = ConfigurationManager.AppSettings["client_secret"];

            if (!string.IsNullOrWhiteSpace(expectedClientId) &&
                string.Equals(expectedClientId, clientId?.Trim(), System.StringComparison.Ordinal) &&
                string.Equals(expectedClientSecret, clientSecret?.Trim(), System.StringComparison.Ordinal))
            {
                context.Validated(clientId);
            }
            else
            {
                context.SetError("invalid_client", "Client authentication failed.");
            }

            return Task.FromResult(0);
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var result = new DAL.DbWrapper().AutenticarUsuario(context.UserName, context.Password);
            if (result == null || !result.IsSuccess || result.Response == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return Task.FromResult(0);
            }

            var usuario = result.Response;
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim("usuarioId", usuario.Id.ToString()));
            if (usuario.EmpresaId > 0)
            {
                identity.AddClaim(new Claim("empresaId", usuario.EmpresaId.ToString()));
            }

            context.Validated(identity);
            return Task.FromResult(0);
        }
    }
}
