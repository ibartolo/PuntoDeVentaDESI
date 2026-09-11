using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Pagina")]
    public class PaginaController : BaseController
    {
        private readonly PaginaService _paginaService;

        public PaginaController()
        {
            _paginaService = new PaginaService();
        }

        /// <summary>Lista todas las páginas/menús activos.</summary>
        [HttpGet, Route("List")]
        public ModelResponse<List<Pagina>> ObtenerPaginas()
        {
            return _paginaService.ObtenerPaginas();
        }

        /// <summary>Obtiene una página por su Id.</summary>
        [HttpGet, Route("{id:long}")]
        public ModelResponse<Pagina> ObtenerPaginaPorId(long id)
        {
            return _paginaService.ObtenerPaginaPorId(id);
        }

        /// <summary>Lista las páginas visibles para un usuario.</summary>
        [HttpGet, Route("Usuario/{usuarioId:long}")]
        public ModelResponse<List<Pagina>> ObtenerPaginasPorUsuario(long usuarioId)
        {
            return _paginaService.ObtenerPaginasPorUsuario(ObtenerEmpresaIdDesdeClaim(), usuarioId);
        }

        /// <summary>Guarda o actualiza una página.</summary>
        [HttpPost, Route("Guardar")]
        public ModelResponse<Pagina> GuardarOActualizarPagina(Pagina pagina)
        {
            return _paginaService.GuardarOActualizarPagina(pagina, User.Identity.Name);
        }

        /// <summary>Desactiva lógicamente una página.</summary>
        [HttpDelete, Route("Eliminar")]
        public ModelResponse EliminarPagina(Pagina pagina)
        {
            return _paginaService.EliminarPagina(pagina.Id, User.Identity.Name);
        }
    }
}
