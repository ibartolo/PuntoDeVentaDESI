using Newtonsoft.Json;
using PuntoDeVentaEntities.Ventas;
using PuntoDeVentaMVC.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PuntoDeVentaMVC.Controllers
{
    /// <summary>
    /// Controlador de cancelaciones/devoluciones de venta. Dos regiones: Views (render) y
    /// Data Access (AJAX contra la WebApi).
    /// </summary>
    public class CancelacionController : BaseController
    {
        private readonly CancelacionService _cancelacionService;

        public CancelacionController()
        {
            _cancelacionService = new CancelacionService(httpClientConnection);
        }

        #region Views

        public async Task<ActionResult> Cancelacion(long id = 0)
        {
            var cancelacion = new CancelacionDTO();

            if (id > 0)
            {
                var cancelacionResponse = await _cancelacionService.ObtenerCancelacionPorId(id);
                if (cancelacionResponse != null)
                {
                    cancelacion = cancelacionResponse;
                }
                else
                {
                    ViewBag.ErrorMessage = "No se encontró la cancelación.";
                }
            }

            ViewBag.DetalleJson = JsonConvert.SerializeObject(cancelacion.Detalle ?? new List<DevolucionDetalleDTO>());

            return View("~/Views/Ventas/Cancelacion.cshtml", cancelacion);
        }

        #endregion

        #region Data Access

        public async Task<string> ConsultarTodasLasCancelaciones(long sucursalId = 0)
        {
            var response = await _cancelacionService.ConsultarTodasLasCancelaciones(sucursalId);
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarCancelacionPorId(long id)
        {
            var response = await _cancelacionService.ObtenerCancelacionPorId(id);
            return JsonConvert.SerializeObject(response);
        }

        /// <summary>
        /// Registra la cancelación. El detalle viaja como JSON (detalleJson) porque jQuery no
        /// serializa listas anidadas al formato de binding de MVC.
        /// </summary>
        [HttpPost]
        public async Task<string> GuardarCancelacion(CancelacionDTO cancelacion, string detalleJson)
        {
            if (!string.IsNullOrWhiteSpace(detalleJson))
            {
                cancelacion.Detalle = JsonConvert.DeserializeObject<List<DevolucionDetalleDTO>>(detalleJson)
                    ?? new List<DevolucionDetalleDTO>();
            }

            var tc = SessionHelperUser();
            if (string.IsNullOrWhiteSpace(cancelacion.UsuarioCancela))
            {
                cancelacion.UsuarioCancela = tc ?? "system";
            }

            cancelacion.CreadoPor = tc ?? "system";
            cancelacion.FechaCreacion = System.DateTime.Now;
            cancelacion.Estatus = true;

            var response = await _cancelacionService.GuardarCancelacion(cancelacion);
            return JsonConvert.SerializeObject(response);
        }

        private string SessionHelperUser()
        {
            return PuntoDeVentaMVC.Helpers.SessionHelper.GetSessionUser()?.UserName;
        }

        #endregion
    }
}
