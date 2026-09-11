using PuntoDeVentaEntities.Reportes;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.DAL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    /// <summary>Servicio del front para los reportes (passthrough hacia la WebApi).</summary>
    public class ReporteService
    {
        private readonly HttpClientConnection _httpClient;

        public ReporteService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ModelResponse<List<ReporteVentasPeriodoDTO>>> ObtenerVentasPorPeriodo(DateTime fechaInicio, DateTime fechaFin, long sucursalId)
        {
            return await _httpClient.ObtenerVentasPorPeriodo(fechaInicio, fechaFin, sucursalId);
        }

        public async Task<ModelResponse<List<ReporteVentasSucursalDTO>>> ObtenerVentasPorSucursal(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _httpClient.ObtenerVentasPorSucursal(fechaInicio, fechaFin);
        }

        public async Task<ModelResponse<List<ReporteVentasCajeroDTO>>> ObtenerVentasPorCajero(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _httpClient.ObtenerVentasPorCajero(fechaInicio, fechaFin);
        }

        public async Task<ModelResponse<List<ReporteUtilidadDTO>>> ObtenerUtilidad(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _httpClient.ObtenerUtilidad(fechaInicio, fechaFin);
        }

        public async Task<ModelResponse<List<ReporteMasVendidosDTO>>> ObtenerMasVendidos(DateTime fechaInicio, DateTime fechaFin, int top)
        {
            return await _httpClient.ObtenerMasVendidos(fechaInicio, fechaFin, top);
        }
    }
}
