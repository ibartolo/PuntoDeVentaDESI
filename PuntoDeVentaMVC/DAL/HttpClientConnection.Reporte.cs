using PuntoDeVentaEntities.Reportes;
using PuntoDeVentaEntities.Seguridad;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.DAL
{
    /// <summary>
    /// Acceso HTTP del front a los endpoints de reportes de la WebApi.
    /// </summary>
    public partial class HttpClientConnection
    {
        public async Task<ModelResponse<List<ReporteVentasPeriodoDTO>>> ObtenerVentasPorPeriodo(DateTime fechaInicio, DateTime fechaFin, long sucursalId)
        {
            var url = string.Format(
                CultureInfo.InvariantCulture,
                "api/Reporte/VentasPorPeriodo?fechaInicio={0}&fechaFin={1}&sucursalId={2}",
                FormatearFecha(fechaInicio),
                FormatearFecha(fechaFin),
                sucursalId);

            return await RequestAsync<List<ReporteVentasPeriodoDTO>>(url, HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<List<ReporteVentasSucursalDTO>>> ObtenerVentasPorSucursal(DateTime fechaInicio, DateTime fechaFin)
        {
            var url = string.Format(
                CultureInfo.InvariantCulture,
                "api/Reporte/VentasPorSucursal?fechaInicio={0}&fechaFin={1}",
                FormatearFecha(fechaInicio),
                FormatearFecha(fechaFin));

            return await RequestAsync<List<ReporteVentasSucursalDTO>>(url, HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<List<ReporteVentasCajeroDTO>>> ObtenerVentasPorCajero(DateTime fechaInicio, DateTime fechaFin)
        {
            var url = string.Format(
                CultureInfo.InvariantCulture,
                "api/Reporte/VentasPorCajero?fechaInicio={0}&fechaFin={1}",
                FormatearFecha(fechaInicio),
                FormatearFecha(fechaFin));

            return await RequestAsync<List<ReporteVentasCajeroDTO>>(url, HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<List<ReporteUtilidadDTO>>> ObtenerUtilidad(DateTime fechaInicio, DateTime fechaFin)
        {
            var url = string.Format(
                CultureInfo.InvariantCulture,
                "api/Reporte/Utilidad?fechaInicio={0}&fechaFin={1}",
                FormatearFecha(fechaInicio),
                FormatearFecha(fechaFin));

            return await RequestAsync<List<ReporteUtilidadDTO>>(url, HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<List<ReporteMasVendidosDTO>>> ObtenerMasVendidos(DateTime fechaInicio, DateTime fechaFin, int top)
        {
            var url = string.Format(
                CultureInfo.InvariantCulture,
                "api/Reporte/MasVendidos?fechaInicio={0}&fechaFin={1}&top={2}",
                FormatearFecha(fechaInicio),
                FormatearFecha(fechaFin),
                top);

            return await RequestAsync<List<ReporteMasVendidosDTO>>(url, HttpMethod.Get, null, token?.Token?.access_token);
        }

        /// <summary>Formatea una fecha al formato ISO-8601 esperado por la WebApi.</summary>
        private static string FormatearFecha(DateTime fecha)
        {
            return fecha.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
