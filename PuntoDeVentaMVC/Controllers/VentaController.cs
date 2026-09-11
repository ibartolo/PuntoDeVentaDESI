using Newtonsoft.Json;
using PuntoDeVentaEntities.Ventas;
using PuntoDeVentaMVC.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PuntoDeVentaMVC.Controllers
{
    /// <summary>
    /// Controlador del punto de venta. Dos regiones: Views (render) y Data Access (AJAX).
    /// </summary>
    public class VentaController : BaseController
    {
        private readonly VentaService _ventaService;

        public VentaController()
        {
            _ventaService = new VentaService(httpClientConnection);
        }

        #region Views

        public async Task<ActionResult> Index(long id = 0)
        {
            var venta = new VentaDTO();

            if (id > 0)
            {
                var ventaResponse = await _ventaService.ObtenerVentaPorId(id);
                if (ventaResponse != null)
                {
                    venta = ventaResponse;
                }
                else
                {
                    ViewBag.ErrorMessage = "No se encontró la venta.";
                }
            }

            return View("~/Views/Ventas/Index.cshtml", venta);
        }

        #endregion

        #region Data Access

        public async Task<string> ConsultarTodasLasVentas(long sucursalId = 0)
        {
            var response = await _ventaService.ConsultarTodasLasVentas(sucursalId);
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarVentaPorId(long id)
        {
            var response = await _ventaService.ObtenerVentaPorId(id);
            return JsonConvert.SerializeObject(response);
        }

        /// <summary>
        /// Registra la venta. El detalle viaja como JSON (detalleJson) porque jQuery no
        /// serializa listas anidadas al formato de binding de MVC.
        /// </summary>
        [HttpPost]
        public async Task<string> GuardarVenta(VentaDTO venta, string detalleJson)
        {
            if (!string.IsNullOrWhiteSpace(detalleJson))
            {
                venta.Detalle = JsonConvert.DeserializeObject<List<VentaDetalleDTO>>(detalleJson)
                    ?? new List<VentaDetalleDTO>();
            }

            var tc = SessionHelperUser();
            venta.CreadoPor = tc ?? "system";
            venta.FechaCreacion = System.DateTime.Now;
            venta.Estatus = true;

            var response = await _ventaService.GuardarVenta(venta);
            return JsonConvert.SerializeObject(response);
        }

        private string SessionHelperUser()
        {
            return PuntoDeVentaMVC.Helpers.SessionHelper.GetSessionUser()?.UserName;
        }

        #endregion
    }
}
