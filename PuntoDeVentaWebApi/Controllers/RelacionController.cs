using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Relacion")]
    public class RelacionController : BaseController
    {
        private readonly UsuarioPaginaService _usuarioPaginaService;

        public RelacionController()
        {
            _usuarioPaginaService = new UsuarioPaginaService();
        }

        /// <summary>Lista todas las relaciones usuario-página activas.</summary>
        [HttpGet, Route("List")]
        public ModelResponse<List<UsuarioPagina>> ObtenerRelaciones()
        {
            return _usuarioPaginaService.ObtenerTodasRelaciones();
        }

        /// <summary>Obtiene una relación usuario-página por su Id.</summary>
        [HttpGet, Route("{id:long}")]
        public ModelResponse<UsuarioPagina> ObtenerRelacionPorId(long id)
        {
            return _usuarioPaginaService.ObtenerRelacionPorId(id);
        }

        /// <summary>Guarda o reactiva una relación usuario-página.</summary>
        [HttpPost, Route("Guardar")]
        public ModelResponse<UsuarioPagina> GuardarOActualizarRelacion(UsuarioPagina relacion)
        {
            return _usuarioPaginaService.GuardarOActualizarRelacion(relacion, User.Identity.Name);
        }

        /// <summary>Desactiva lógicamente una relación usuario-página.</summary>
        [HttpDelete, Route("Eliminar")]
        public ModelResponse EliminarRelacion(UsuarioPagina relacion)
        {
            return _usuarioPaginaService.EliminarUsuarioPagina(relacion.Id, User.Identity.Name);
        }
    }
}
