using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Filters;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Rol")]
    public class RolController : BaseController
    {
        private readonly RolService _rolService;

        public RolController()
        {
            _rolService = new RolService();
        }

        /// <summary>Lista los roles de la empresa del usuario autenticado.</summary>
        [HttpGet, Route("List")]
        public ModelResponse<List<Rol>> ObtenerRoles()
        {
            return _rolService.ObtenerRoles(User.Identity.Name);
        }

        /// <summary>Obtiene un rol por su Id.</summary>
        [HttpGet, Route("{id:long}")]
        public ModelResponse<Rol> ObtenerRolPorId(long id)
        {
            return _rolService.ObtenerRolPorId(ObtenerEmpresaIdDesdeClaim(), id);
        }

        /// <summary>Lista los roles asignados a un usuario.</summary>
        [HttpGet, Route("Usuario/{usuarioId:long}")]
        public ModelResponse<List<Rol>> ObtenerRolesPorUsuario(long usuarioId)
        {
            return _rolService.ObtenerRolesPorUsuario(ObtenerEmpresaIdDesdeClaim(), usuarioId);
        }

        /// <summary>Guarda o actualiza un rol.</summary>
        [Permiso("Roles", "Editar")]
        [HttpPost, Route("Guardar")]
        public ModelResponse<Rol> GuardarOActualizarRol(Rol rol)
        {
            return _rolService.GuardarOActualizarRol(ObtenerEmpresaIdDesdeClaim(), rol, User.Identity.Name);
        }

        /// <summary>Desactiva lógicamente un rol.</summary>
        [Permiso("Roles", "Eliminar")]
        [HttpDelete, Route("Eliminar")]
        public ModelResponse EliminarRol(Rol rol)
        {
            return _rolService.EliminarRol(ObtenerEmpresaIdDesdeClaim(), rol.Id, User.Identity.Name);
        }

        /// <summary>Asigna un rol a un usuario.</summary>
        [Permiso("Roles", "Crear")]
        [HttpPost, Route("Asignar")]
        public ModelResponse AsignarRolUsuario([FromBody] AsignarRolRequest request)
        {
            return dbWrapper.AsignarRolUsuario(ObtenerEmpresaIdDesdeClaim(), request.UsuarioId, request.RolId, User.Identity.Name);
        }

        /// <summary>Lista las asignaciones usuario-rol (junction) de un usuario.</summary>
        [HttpGet, Route("UsuarioRoles/{usuarioId:long}")]
        public ModelResponse<List<UsuarioRol>> ObtenerUsuarioRolesPorUsuario(long usuarioId)
        {
            return dbWrapper.ObtenerUsuarioRolesPorUsuario(ObtenerEmpresaIdDesdeClaim(), usuarioId);
        }

        /// <summary>Elimina la asignación de un rol a un usuario.</summary>
        [Permiso("Roles", "Eliminar")]
        [HttpDelete, Route("EliminarUsuarioRol")]
        public ModelResponse EliminarRolUsuario([FromBody] EliminarRolUsuarioRequest request)
        {
            return dbWrapper.EliminarUsuarioRol(ObtenerEmpresaIdDesdeClaim(), request.UsuarioRolId, User.Identity.Name);
        }
    }

    /// <summary>Petición de asignación de rol a un usuario.</summary>
    public class AsignarRolRequest
    {
        public long UsuarioId { get; set; }
        public long RolId { get; set; }
    }

    /// <summary>Petición de eliminación de una asignación usuario-rol.</summary>
    public class EliminarRolUsuarioRequest
    {
        public long UsuarioRolId { get; set; }
    }
}
