using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Filters
{
    /// <summary>
    /// Atributo que valida el permiso (página/acción) del usuario autenticado contra
    /// RolPaginaAccion. La autenticación (401) la garantiza el <c>[Authorize]</c> a nivel de
    /// controlador; aquí sólo se valida el permiso y se responde 403 si no lo tiene.
    /// Se implementa como action filter (posterior al model binding) para poder deducir la
    /// acción desde el Id de la entidad cuando no se especifica explícitamente.
    /// La empresa y el usuario se leen de los claims del token (empresaId/usuarioId), igual
    /// que <see cref="Controllers.BaseController.ObtenerEmpresaIdDesdeClaim"/>.
    /// </summary>
    public class PermisoAttribute : ActionFilterAttribute
    {
        private readonly string _pagina;
        private readonly string _accion;

        /// <summary>
        /// Constructor con acción explícita (p. ej. "Crear", "Editar", "Eliminar").
        /// </summary>
        /// <param name="pagina">Nombre exacto de la página (Pagina.Nombre).</param>
        /// <param name="accion">Acción requerida (Crear/Editar/Eliminar/Leer/Exportar).</param>
        public PermisoAttribute(string pagina, string accion)
        {
            _pagina = pagina;
            _accion = accion;
        }

        /// <summary>
        /// Constructor sin acción: la acción se auto-detecta en OnActionExecuting.
        /// DELETE -> "Eliminar"; entidad bound con Id == 0 -> "Crear"; con Id > 0 -> "Editar".
        /// </summary>
        /// <param name="pagina">Nombre exacto de la página (Pagina.Nombre).</param>
        public PermisoAttribute(string pagina) : this(pagina, null)
        {
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var empresaId = ObtenerClaimLong(actionContext, "empresaId");
            var usuarioId = ObtenerClaimLong(actionContext, "usuarioId");
            var accion = ResolverAccion(actionContext);

            var resultado = new PermisosService().ValidarPermisoUsuario(empresaId, usuarioId, _pagina, accion);

            if (resultado == null || !resultado.IsSuccess || !resultado.Response)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Forbidden,
                    "No tiene permiso para realizar esta acción.");
            }
        }

        /// <summary>Lee un claim numérico (empresaId/usuarioId) del principal del request.</summary>
        private static long ObtenerClaimLong(HttpActionContext actionContext, string tipo)
        {
            var identity = actionContext.RequestContext.Principal?.Identity as ClaimsIdentity;
            var claim = identity?.FindFirst(tipo);
            long valor;
            if (claim != null && long.TryParse(claim.Value, out valor))
            {
                return valor;
            }

            return 0;
        }

        /// <summary>
        /// Resuelve la acción a validar. Prioridad: acción explícita > DELETE ("Eliminar")
        /// > entidad bound con Id numérico (0 -> "Crear", > 0 -> "Editar") > null (se deniega).
        /// </summary>
        private string ResolverAccion(HttpActionContext actionContext)
        {
            if (!string.IsNullOrWhiteSpace(_accion))
            {
                return _accion;
            }

            if (actionContext.Request.Method == HttpMethod.Delete)
            {
                return "Eliminar";
            }

            foreach (var argumento in actionContext.ActionArguments.Values)
            {
                if (argumento == null)
                {
                    continue;
                }

                var propiedadId = argumento.GetType().GetProperty("Id");
                if (propiedadId != null && EsNumerico(propiedadId.PropertyType))
                {
                    var idValor = Convert.ToInt64(propiedadId.GetValue(argumento));
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
