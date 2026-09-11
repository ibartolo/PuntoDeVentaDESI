using System.Web.Http;

namespace PuntoDeVenta.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(App_Start.WebApiConfig.Register);
        }
    }
}
