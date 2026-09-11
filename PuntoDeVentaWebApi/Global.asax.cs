using System.Web.Http;
using Serilog;

namespace PuntoDeVentaWebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(App_Start.WebApiConfig.Register);
        }

        protected void Application_End()
        {
            Log.CloseAndFlush();
        }
    }
}
