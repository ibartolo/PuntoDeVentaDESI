using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Filters;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Permisos")]
    public class PermisosController : BaseController
    {
        private readonly PermisosService _permisosService;

        public PermisosController()
        {
            _permisosService = new PermisosService();
        }

        /// <summary>Obtiene todos los permisos del usuario autenticado.</summary>
        [HttpGet, Route("List")]
        public ModelResponse<List<PermisosViewModel>> ObtenerPermisosPorUsuario()
        {
            return _permisosService.ObtenerPermisosPorUsuario(User.Identity.Name);
        }

        /// <summary>Valida si el usuario autenticado tiene un permiso sobre una página.</summary>
        [HttpPost, Route("Validar")]
        public ModelResponse<bool> ValidarPermisoUsuario([FromBody] ValidarPermisoRequest request)
        {
            if (request == null)
            {
                return new ModelResponse<bool> { IsSuccess = false, Message = "La solicitud es requerida." };
            }

            return _permisosService.ValidarPermisoUsuario(
                ObtenerEmpresaIdDesdeClaim(),
                ObtenerUsuarioIdDesdeClaim(),
                request.NombrePagina,
                request.Accion);
        }

        /// <summary>Obtiene todas las páginas del sistema.</summary>
        [HttpGet, Route("Paginas")]
        public ModelResponse<List<Pagina>> ObtenerPaginas()
        {
            return _permisosService.ObtenerPaginas();
        }

        /// <summary>Obtiene los permisos de un rol específico.</summary>
        [HttpGet, Route("Rol/{rolId:long}")]
        public ModelResponse<List<RolPaginaAccionDTO>> ObtenerPermisosPorRol(long rolId)
        {
            return _permisosService.ObtenerPermisosPorRol(ObtenerEmpresaIdDesdeClaim(), rolId);
        }

        /// <summary>Guarda los permisos de un rol sobre una página.</summary>
        [Permiso("Permisos", "Editar")]
        [HttpPost, Route("Guardar")]
        public ModelResponse GuardarPermisosRol([FromBody] GuardarPermisosRequest request)
        {
            if (request == null)
            {
                return new ModelResponse { IsSuccess = false, Message = "La solicitud es requerida." };
            }

            return _permisosService.GuardarPermisosRol(
                ObtenerEmpresaIdDesdeClaim(),
                request.RolId,
                request.PaginaId,
                request.PuedeLeer,
                request.PuedeCrear,
                request.PuedeEditar,
                request.PuedeEliminar,
                request.PuedeExportar,
                User.Identity.Name);
        }

        /// <summary>Guarda todos los permisos de un rol de forma masiva (transaccional).</summary>
        [Permiso("Permisos", "Editar")]
        [HttpPost, Route("GuardarMasivo")]
        public ModelResponse GuardarPermisosRolMasivo([FromBody] GuardarPermisosMasivoRequest request)
        {
            if (request == null)
            {
                return new ModelResponse { IsSuccess = false, Message = "La solicitud es requerida." };
            }

            return _permisosService.GuardarPermisosRolMasivo(
                ObtenerEmpresaIdDesdeClaim(),
                request.RolId,
                request.Permisos,
                User.Identity.Name);
        }

        /// <summary>Obtiene el conteo de páginas asignadas por rol.</summary>
        [HttpGet, Route("ConteoPaginasPorRol")]
        public ModelResponse<List<RolConteoPaginasDTO>> ObtenerConteoPaginasPorRol()
        {
            return _permisosService.ObtenerConteoPaginasPorRol();
        }
    }
}
