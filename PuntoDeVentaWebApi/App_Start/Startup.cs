using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Serilog;

[assembly: OwinStartup(typeof(PuntoDeVentaWebApi.App_Start.Startup))]

namespace PuntoDeVentaWebApi.App_Start
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthServerOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            ConfigureLogging();
            ConfigureCors(app);
            ConfigureOAuth(app);

            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            SwaggerConfig.Register(config);
            app.UseWebApi(config);
        }

        private static void ConfigureLogging()
        {
            var logPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/logs/log-.txt");
            if (string.IsNullOrEmpty(logPath))
            {
                logPath = "App_Data/logs/log-.txt";
            }

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 31)
                .CreateLogger();
        }

        private static void ConfigureCors(IAppBuilder app)
        {
            var allowedOrigins = (ConfigurationManager.AppSettings["AllowedCorsOrigins"] ?? string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            app.Use(async (context, next) =>
            {
                var origin = context.Request.Headers["Origin"];
                if (!string.IsNullOrEmpty(origin))
                {
                    context.Response.Headers.Set("Access-Control-Allow-Origin", origin);
                }
                else if (allowedOrigins.Length > 0)
                {
                    context.Response.Headers.Set("Access-Control-Allow-Origin", allowedOrigins[0].Trim());
                }

                context.Response.Headers.Set("Access-Control-Allow-Headers", "Authorization,Content-Type");
                context.Response.Headers.Set("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,OPTIONS");

                if (string.Equals(context.Request.Method, "OPTIONS", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = 200;
                    return;
                }

                await next();
            });
        }

        private static void ConfigureOAuth(IAppBuilder app)
        {
            bool allowInsecure;
            if (!bool.TryParse(ConfigurationManager.AppSettings["AllowInsecureHttp"], out allowInsecure))
            {
                allowInsecure = true;
            }

            OAuthServerOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = allowInsecure,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(6),
                Provider = new TokenAuthorizationServerProvider()
            };

            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
