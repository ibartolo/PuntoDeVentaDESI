using Newtonsoft.Json;
using PuntoDeVentaEntities.Caja;
using PuntoDeVentaMVC.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PuntoDeVentaMVC.Controllers
{
    /// <summary>
    /// Controlador de caja chica. Dos regiones: Views (render) y Data Access (AJAX).
    /// </summary>
    public class CajaChicaController : BaseController
    {
        private readonly CajaChicaService _cajaChicaService;

        public CajaChicaController()
        {
            _cajaChicaService = new CajaChicaService(httpClientConnection);
        }

        #region Views

        public async Task<ActionResult> Index()
        {
            var caja = new CajaChicaDTO();

            if (tokenCookie != null && tokenCookie.SucursalID > 0 && tokenCookie.UserID > 0)
            {
                var abierta = await _cajaChicaService.ObtenerCajaChicaAbierta(tokenCookie.SucursalID, tokenCookie.UserID);
                if (abierta != null)
                {
                    caja = abierta;
                }
            }
            else
            {
                ViewBag.ErrorMessage = "No se pudo resolver la sucursal o el usuario de la sesión.";
            }

            return View("~/Views/Caja/Index.cshtml", caja);
        }

        #endregion

        #region Data Access

        public async Task<string> ConsultarCajaChicaAbierta(long sucursalId, long usuarioId)
        {
            var response = await _cajaChicaService.ConsultarCajaChicaAbierta(sucursalId, usuarioId);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> AbrirCajaChica(CajaChicaDTO caja)
        {
            if (caja.SucursalId <= 0 && tokenCookie != null)
            {
                caja.SucursalId = tokenCookie.SucursalID;
            }

            if (caja.UsuarioId <= 0 && tokenCookie != null)
            {
                caja.UsuarioId = tokenCookie.UserID;
            }

            var tc = SessionHelperUser();
            caja.CreadoPor = tc ?? "system";
            caja.FechaCreacion = System.DateTime.Now;
            caja.Estatus = true;

            var response = await _cajaChicaService.AbrirCajaChica(caja);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> CerrarCajaChica(long id)
        {
            var caja = new CajaChicaDTO
            {
                Id = id,
                ModificadoPor = SessionHelperUser() ?? "system",
                FechaModificacion = System.DateTime.Now
            };

            var response = await _cajaChicaService.CerrarCajaChica(caja);
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarSalidasCaja(long cajaChicaId)
        {
            var response = await _cajaChicaService.ConsultarSalidasCaja(cajaChicaId);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> RegistrarSalidaCaja(SalidaCaja salida)
        {
            var tc = SessionHelperUser();
            salida.CreadoPor = tc ?? "system";
            salida.FechaCreacion = System.DateTime.Now;
            salida.Estatus = true;

            var response = await _cajaChicaService.RegistrarSalidaCaja(salida);
            return JsonConvert.SerializeObject(response);
        }

        private string SessionHelperUser()
        {
            return PuntoDeVentaMVC.Helpers.SessionHelper.GetSessionUser()?.UserName;
        }

        #endregion
    }
}
