using Newtonsoft.Json;
using PuntoDeVentaEntities.Caja;
using PuntoDeVentaMVC.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PuntoDeVentaMVC.Controllers
{
    /// <summary>
    /// Controlador de cortes de caja. Dos regiones: Views (render) y Data Access (AJAX).
    /// El corte cierra la caja chica y resume las ventas por método de pago.
    /// </summary>
    public class CorteController : BaseController
    {
        private readonly CorteService _corteService;

        public CorteController()
        {
            _corteService = new CorteService(httpClientConnection);
        }

        #region Views

        public async Task<ActionResult> Corte(long id = 0)
        {
            var corte = new CorteDTO();

            if (id > 0)
            {
                var corteResponse = await _corteService.ObtenerCortePorId(id);
                if (corteResponse != null)
                {
                    corte = corteResponse;
                }
                else
                {
                    ViewBag.ErrorMessage = "No se encontró el corte.";
                }
            }
            else
            {
                // Prellenado desde la sesión para el alta de un corte nuevo.
                corte.SucursalId = tokenCookie?.SucursalID ?? 0;
                corte.UsuarioId = tokenCookie?.UserID ?? 0;
            }

            return View("~/Views/Caja/Corte.cshtml", corte);
        }

        #endregion

        #region Data Access

        public async Task<string> ConsultarTodosLosCortes(long sucursalId = 0)
        {
            var response = await _corteService.ConsultarTodosLosCortes(sucursalId);
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarCortePorId(long id)
        {
            var response = await _corteService.ConsultarCortePorId(id);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> GuardarCorte(CorteDTO corte)
        {
            var tc = SessionHelperUser();
            corte.CreadoPor = tc ?? "system";
            corte.FechaCreacion = System.DateTime.Now;
            corte.Estatus = true;

            if (corte.UsuarioId <= 0)
            {
                corte.UsuarioId = tokenCookie?.UserID ?? 0;
            }

            if (corte.SucursalId <= 0)
            {
                corte.SucursalId = tokenCookie?.SucursalID ?? 0;
            }

            var response = await _corteService.GuardarCorte(corte);
            return JsonConvert.SerializeObject(response);
        }

        private string SessionHelperUser()
        {
            return PuntoDeVentaMVC.Helpers.SessionHelper.GetSessionUser()?.UserName;
        }

        #endregion
    }
}
