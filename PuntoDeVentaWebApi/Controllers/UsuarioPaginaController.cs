using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/UsuarioPagina")]
    public class UsuarioPaginaController : BaseController
    {
        private readonly UsuarioPaginaService _usuarioPaginaService;

        public UsuarioPaginaController()
        {
            _usuarioPaginaService = new UsuarioPaginaService();
        }

        /// <summary>Lista todos los accesos directos usuario-página activos.</summary>
        [HttpGet, Route("List")]
        public ModelResponse<List<UsuarioPagina>> ObtenerUsuarioPagina()
        {
            return _usuarioPaginaService.ObtenerTodasRelaciones();
        }

        /// <summary>Lista los accesos directos a páginas de un usuario.</summary>
        [HttpGet, Route("Usuario/{usuarioId:long}")]
        public ModelResponse<List<UsuarioPagina>> ObtenerUsuarioPaginaPorUsuario(long usuarioId)
        {
            return _usuarioPaginaService.ObtenerUsuarioPaginaPorUsuario(ObtenerEmpresaIdDesdeClaim(), usuarioId);
        }

        /// <summary>Obtiene un acceso directo por su Id.</summary>
        [HttpGet, Route("{id:long}")]
        public ModelResponse<UsuarioPagina> ObtenerUsuarioPaginaPorId(long id)
        {
            return _usuarioPaginaService.ObtenerRelacionPorId(id);
        }

        /// <summary>Guarda o reactiva un acceso directo usuario-página.</summary>
        [HttpPost, Route("Guardar")]
        public ModelResponse<UsuarioPagina> GuardarOActualizarUsuarioPagina(UsuarioPagina relacion)
        {
            return _usuarioPaginaService.GuardarOActualizarRelacion(relacion, User.Identity.Name);
        }

        /// <summary>Desactiva lógicamente un acceso directo usuario-página.</summary>
        [HttpDelete, Route("Eliminar")]
        public ModelResponse EliminarUsuarioPagina(UsuarioPagina relacion)
        {
            return _usuarioPaginaService.EliminarUsuarioPagina(relacion.Id, User.Identity.Name);
        }
    }
}
