using System.Web.Mvc;
using System.Web.Routing;
using PuntoDeVentaMVC.App_Start;
using Serilog;

namespace PuntoDeVentaMVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ConfigureLogging();
        }

        protected void Application_End()
        {
            Log.CloseAndFlush();
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
    }
}
