using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Precio")]
    public class PrecioController : BaseController
    {
        private readonly PrecioService _precioService;

        public PrecioController()
        {
            _precioService = new PrecioService();
        }

        [HttpGet, Route("Activo/{productoId:long}")]
        public ModelResponse<PrecioDTO> ObtenerPrecioActivo(long productoId)
        {
            return _precioService.ObtenerPrecioActivo(ObtenerEmpresaIdDesdeClaim(), productoId);
        }

        [HttpGet, Route("PorProducto/{productoId:long}")]
        public ModelResponse<List<PrecioDTO>> ObtenerPreciosPorProducto(long productoId)
        {
            return _precioService.ObtenerPreciosPorProducto(ObtenerEmpresaIdDesdeClaim(), productoId);
        }

        [HttpPost, Route("Guardar")]
        public ModelResponse<PrecioDTO> GuardarOActualizarPrecio(Precio precio)
        {
            return _precioService.GuardarOActualizarPrecio(ObtenerEmpresaIdDesdeClaim(), precio, User.Identity.Name);
        }
    }
}
