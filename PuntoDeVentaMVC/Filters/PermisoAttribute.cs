using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using PuntoDeVentaMVC.DAL;
using PuntoDeVentaMVC.Services;

namespace PuntoDeVentaMVC.Filters
{
    /// <summary>
    /// Atributo MVC que fuerza la validación de permisos contra RolPaginaAccion.
    /// La sesión la garantiza el filtro global AuthenticationFilter; aquí se valida el
    /// permiso y, si falta, se redirige a Home/AccesoDenegado.
    /// </summary>
    public class PermisoAttribute : ActionFilterAttribute
    {
        private readonly string _pagina;
        private readonly string _accion;

        public PermisoAttribute(string pagina, string accion)
        {
            _pagina = pagina;
            _accion = accion;
        }

        public PermisoAttribute(string pagina) : this(pagina, null)
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }

            var accion = ResolverAccion(filterContext);

            // HttpClientConnection lee la sesión de HttpContext.Current en su constructor;
            // por eso el service se construye aquí (hilo de la petición) y solo la espera
            // async corre dentro de Task.Run.
            var permisosService = new PermisosService(new HttpClientConnection());

            var permitido = Task.Run(() => permisosService.TienePermiso(_pagina, accion))
                .GetAwaiter()
                .GetResult();

            if (!permitido)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "AccesoDenegado"
                }));
            }
        }

        private string ResolverAccion(ActionExecutingContext filterContext)
        {
            if (!string.IsNullOrWhiteSpace(_accion))
            {
                return _accion;
            }

            if (string.Equals(filterContext.HttpContext.Request.HttpMethod, "DELETE", StringComparison.OrdinalIgnoreCase))
            {
                return "Eliminar";
            }

            foreach (var parametro in filterContext.ActionParameters.Values)
            {
                if (parametro == null)
                {
                    continue;
                }

                var propiedadId = parametro.GetType().GetProperty("Id");
                if (propiedadId != null && EsNumerico(propiedadId.PropertyType))
                {
                    var idValor = Convert.ToInt64(propiedadId.GetValue(parametro));
                    return idValor == 0 ? "Crear" : "Editar";
                }
            }

            return null;
        }

        private static bool EsNumerico(Type tipo)
        {
            if (tipo == null)
            {
                return false;
            }

            var tipoBase = Nullable.GetUnderlyingType(tipo) ?? tipo;

            return tipoBase == typeof(byte) ||
                   tipoBase == typeof(sbyte) ||
                   tipoBase == typeof(short) ||
                   tipoBase == typeof(ushort) ||
                   tipoBase == typeof(int) ||
                   tipoBase == typeof(uint) ||
                   tipoBase == typeof(long) ||
                   tipoBase == typeof(ulong);
        }
    }
}
