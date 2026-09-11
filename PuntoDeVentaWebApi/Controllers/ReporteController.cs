using System;
using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Reportes;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    /// <summary>
    /// Endpoints de reportes del POS. Todos son de solo lectura y filtran por el
    /// tenant (empresaId) resuelto desde el token.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Reporte")]
    public class ReporteController : BaseController
    {
        private readonly ReporteService _reporteService;

        public ReporteController()
        {
            _reporteService = new ReporteService();
        }

        /// <summary>Ventas detalladas en un rango de fechas (filtro opcional por sucursal).</summary>
        [HttpGet, Route("VentasPorPeriodo")]
        public ModelResponse<List<ReporteVentasPeriodoDTO>> ObtenerVentasPorPeriodo(DateTime fechaInicio, DateTime fechaFin, long sucursalId = 0)
        {
            return _reporteService.ObtenerVentasPorPeriodo(ObtenerEmpresaIdDesdeClaim(), fechaInicio, fechaFin, sucursalId);
        }

        /// <summary>Totales de venta agrupados por sucursal.</summary>
        [HttpGet, Route("VentasPorSucursal")]
        public ModelResponse<List<ReporteVentasSucursalDTO>> ObtenerVentasPorSucursal(DateTime fechaInicio, DateTime fechaFin)
        {
            return _reporteService.ObtenerVentasPorSucursal(ObtenerEmpresaIdDesdeClaim(), fechaInicio, fechaFin);
        }

        /// <summary>Totales de venta agrupados por cajero.</summary>
        [HttpGet, Route("VentasPorCajero")]
        public ModelResponse<List<ReporteVentasCajeroDTO>> ObtenerVentasPorCajero(DateTime fechaInicio, DateTime fechaFin)
        {
            return _reporteService.ObtenerVentasPorCajero(ObtenerEmpresaIdDesdeClaim(), fechaInicio, fechaFin);
        }

        /// <summary>Utilidad por producto (venta - costo).</summary>
        [HttpGet, Route("Utilidad")]
        public ModelResponse<List<ReporteUtilidadDTO>> ObtenerUtilidad(DateTime fechaInicio, DateTime fechaFin)
        {
            return _reporteService.ObtenerUtilidad(ObtenerEmpresaIdDesdeClaim(), fechaInicio, fechaFin);
        }

        /// <summary>Productos más vendidos (top N, por defecto 10).</summary>
        [HttpGet, Route("MasVendidos")]
        public ModelResponse<List<ReporteMasVendidosDTO>> ObtenerMasVendidos(DateTime fechaInicio, DateTime fechaFin, int top = 10)
        {
            return _reporteService.ObtenerMasVendidos(ObtenerEmpresaIdDesdeClaim(), fechaInicio, fechaFin, top);
        }
    }
}
