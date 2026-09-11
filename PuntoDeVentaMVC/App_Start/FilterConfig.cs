using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using PuntoDeVentaMVC.Helpers;

namespace PuntoDeVentaMVC.App_Start
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthenticationFilter());
        }
    }

    /// <summary>Filtro global de autenticación con allow-list de acciones públicas.</summary>
    public class AuthenticationFilter : IAuthorizationFilter
    {
        private static readonly HashSet<string> PublicActions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Home.Autentication",
            "Home.LogIn",
            "Home.RecoverPassword",
            "Home.ValidarToken",
            "Home.RestablecerContrasenia",
            "Home.NewCompany",
            "Home.GuardarNuevaEmpresa",
            "Home.AccesoDenegado",
            "Home.ValidarRecetearContrasenia"
        };

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var key = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "." +
                      filterContext.ActionDescriptor.ActionName;

            if (PublicActions.Contains(key))
            {
                return;
            }

            if (!SessionHelper.ExisteSession())
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
                    new { controller = "Home", action = "Autentication" }));
            }
        }
    }
}
