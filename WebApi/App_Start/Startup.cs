using System;
using System.Configuration;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(PuntoDeVenta.WebApi.App_Start.Startup))]

namespace PuntoDeVenta.WebApi.App_Start
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthServerOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
        }

        private static void ConfigureOAuth(IAppBuilder app)
        {
            OAuthServerOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = bool.TryParse(ConfigurationManager.AppSettings["AllowInsecureHttp"], out var allowInsecure) && allowInsecure,
                TokenEndpointPath = new PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(6),
                Provider = new TokenAuthorizationServerProvider()
            };

            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
