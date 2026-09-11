using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using PuntoDeVenta.WebApi.Services.Auth;

namespace PuntoDeVenta.WebApi.App_Start
{
    public class TokenAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly AuthService _authService;

        public TokenAuthorizationServerProvider()
        {
            _authService = new AuthService();
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.TryGetFormCredentials(out var clientId, out var clientSecret);

            if (_authService.IsValidClient(clientId, clientSecret))
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
            var user = _authService.Authenticate(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return Task.FromResult(0);
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", user.NombreUsuario));
            identity.AddClaim(new Claim("empresaId", user.EmpresaId.ToString()));

            var properties = new AuthenticationProperties();
            var ticket = new AuthenticationTicket(identity, properties);
            context.Validated(ticket);

            return Task.FromResult(0);
        }
    }
}
