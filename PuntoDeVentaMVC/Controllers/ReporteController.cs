using Newtonsoft.Json;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.Services;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PuntoDeVentaMVC.Controllers
{
    /// <summary>
    /// Controlador de reportes del POS. Dos regiones: Views (render) y Data Access (AJAX).
    /// Los reportes son de solo lectura; el filtro de fechas viaja como texto yyyy-MM-dd.
    /// </summary>
    public class ReporteController : BaseController
    {
        /// <summary>Formato de fecha que envía la vista (input type="date").</summary>
        private const string FormatoFecha = "yyyy-MM-dd";

        private readonly ReporteService _reporteService;

        public ReporteController()
        {
            _reporteService = new ReporteService(httpClientConnection);
        }

        #region Views

        public ActionResult Index()
        {
            return View("~/Views/Reportes/Index.cshtml");
        }

        #endregion

        #region Data Access

        [HttpGet]
        public async Task<string> ConsultarVentasPorPeriodo(string fechaInicio, string fechaFin, long sucursalId = 0)
        {
            DateTime inicio;
            DateTime fin;
            if (!TryParseRango(fechaInicio, fechaFin, out inicio, out fin))
            {
                return ErrorRango();
            }

            var response = await _reporteService.ObtenerVentasPorPeriodo(inicio, fin, sucursalId);
            return JsonConvert.SerializeObject(response);
        }

        [HttpGet]
        public async Task<string> ConsultarVentasPorSucursal(string fechaInicio, string fechaFin)
        {
            DateTime inicio;
            DateTime fin;
            if (!TryParseRango(fechaInicio, fechaFin, out inicio, out fin))
            {
                return ErrorRango();
            }

            var response = await _reporteService.ObtenerVentasPorSucursal(inicio, fin);
            return JsonConvert.SerializeObject(response);
        }

        [HttpGet]
        public async Task<string> ConsultarVentasPorCajero(string fechaInicio, string fechaFin)
        {
            DateTime inicio;
            DateTime fin;
            if (!TryParseRango(fechaInicio, fechaFin, out inicio, out fin))
            {
                return ErrorRango();
            }

            var response = await _reporteService.ObtenerVentasPorCajero(inicio, fin);
            return JsonConvert.SerializeObject(response);
        }

        [HttpGet]
        public async Task<string> ConsultarUtilidad(string fechaInicio, string fechaFin)
        {
            DateTime inicio;
            DateTime fin;
            if (!TryParseRango(fechaInicio, fechaFin, out inicio, out fin))
            {
                return ErrorRango();
            }

            var response = await _reporteService.ObtenerUtilidad(inicio, fin);
            return JsonConvert.SerializeObject(response);
        }

        [HttpGet]
        public async Task<string> ConsultarMasVendidos(string fechaInicio, string fechaFin, int top = 10)
        {
            DateTime inicio;
            DateTime fin;
            if (!TryParseRango(fechaInicio, fechaFin, out inicio, out fin))
            {
                return ErrorRango();
            }

            var response = await _reporteService.ObtenerMasVendidos(inicio, fin, top);
            return JsonConvert.SerializeObject(response);
        }

        /// <summary>
        /// Convierte las fechas de la vista en un rango inclusivo: el fin se extiende al
        /// último instante del día para no perder ventas registradas con hora.
        /// </summary>
        private static bool TryParseRango(string fechaInicio, string fechaFin, out DateTime inicio, out DateTime fin)
        {
            inicio = default(DateTime);
            fin = default(DateTime);

            if (string.IsNullOrWhiteSpace(fechaInicio) || string.IsNullOrWhiteSpace(fechaFin))
            {
                return false;
            }

            if (!DateTime.TryParseExact(fechaInicio, FormatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out inicio))
            {
                return false;
            }

            if (!DateTime.TryParseExact(fechaFin, FormatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out fin))
            {
                return false;
            }

            if (inicio > fin)
            {
                return false;
            }

            fin = fin.Date.AddDays(1).AddTicks(-1);
            return true;
        }

        /// <summary>Respuesta estándar ante un rango de fechas inválido.</summary>
        private static string ErrorRango()
        {
            return JsonConvert.SerializeObject(new ModelResponse
            {
                IsSuccess = false,
                Message = "El rango de fechas es inválido. Verifique que la fecha de inicio no sea mayor que la fecha de fin."
            });
        }

        #endregion
    }
}
