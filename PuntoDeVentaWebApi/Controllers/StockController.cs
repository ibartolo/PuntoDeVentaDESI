using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Inventario;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Stock")]
    public class StockController : BaseController
    {
        private readonly StockService _stockService;

        public StockController()
        {
            _stockService = new StockService();
        }

        [HttpGet, Route("Sucursal/{sucursalId:long}")]
        public ModelResponse<List<StockDTO>> ObtenerStockPorSucursal(long sucursalId)
        {
            return _stockService.ObtenerStockPorSucursal(ObtenerEmpresaIdDesdeClaim(), sucursalId);
        }

        [HttpGet, Route("Sucursal/{sucursalId:long}/Producto/{productoId:long}")]
        public ModelResponse<StockDTO> ObtenerStock(long sucursalId, long productoId)
        {
            return _stockService.ObtenerStock(ObtenerEmpresaIdDesdeClaim(), sucursalId, productoId);
        }

        [HttpGet, Route("Movimientos")]
        public ModelResponse<List<StockMovimientoDTO>> ObtenerMovimientos(long sucursalId = 0, long productoId = 0)
        {
            return _stockService.ObtenerMovimientos(ObtenerEmpresaIdDesdeClaim(), sucursalId, productoId);
        }

        [HttpPost, Route("Movimiento")]
        public ModelResponse<decimal> RegistrarMovimiento(StockMovimientoDTO movimiento)
        {
            return _stockService.RegistrarMovimientoStock(ObtenerEmpresaIdDesdeClaim(), movimiento.SucursalId,
                movimiento.ProductoId, movimiento.TipoMovimiento, movimiento.Cantidad, movimiento.Motivo,
                movimiento.ReferenciaId, User.Identity.Name);
        }
    }
}
