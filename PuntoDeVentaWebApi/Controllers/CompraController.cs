using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Compras;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Compra")]
    public class CompraController : BaseController
    {
        private readonly CompraService _compraService;

        public CompraController()
        {
            _compraService = new CompraService();
        }

        [HttpGet, Route("List")]
        public ModelResponse<List<CompraDTO>> ObtenerCompras(long sucursalId = 0)
        {
            return _compraService.ObtenerCompras(ObtenerEmpresaIdDesdeClaim(), sucursalId);
        }

        [HttpGet, Route("{id:long}")]
        public ModelResponse<CompraDTO> ObtenerCompraPorId(long id)
        {
            return _compraService.ObtenerCompraPorId(ObtenerEmpresaIdDesdeClaim(), id);
        }

        [HttpPost, Route("Guardar")]
        public ModelResponse<CompraDTO> GuardarCompra(CompraDTO compra)
        {
            return _compraService.GuardarCompra(ObtenerEmpresaIdDesdeClaim(), compra, User.Identity.Name);
        }

        [HttpDelete, Route("Eliminar")]
        public ModelResponse EliminarCompra(CompraDTO compra)
        {
            return _compraService.EliminarCompra(ObtenerEmpresaIdDesdeClaim(), compra.Id, User.Identity.Name);
        }
    }
}
