using Newtonsoft.Json;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaMVC.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PuntoDeVentaMVC.Controllers
{
    /// <summary>
    /// Controlador del catálogo de Marcas. Dos regiones: Views (render) y Data Access (AJAX).
    /// </summary>
    public class MarcaController : BaseController
    {
        private readonly MarcaService _marcaService;

        public MarcaController()
        {
            _marcaService = new MarcaService(httpClientConnection);
        }

        #region Views

        public async Task<ActionResult> Mark(long id = 0)
        {
            var marca = new Marca();

            if (id > 0)
            {
                var marcaResponse = await _marcaService.ObtenerMarcaPorId(id);
                if (marcaResponse != null)
                {
                    marca = marcaResponse;
                }
                else
                {
                    ViewBag.ErrorMessage = "No se encontró la marca.";
                }
            }

            return View("~/Views/Catalogs/Mark.cshtml", marca);
        }

        #endregion

        #region Data Access

        public async Task<string> ConsultarTodasLasMarcas()
        {
            var response = await _marcaService.ConsultarTodasLasMarcas();
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarMarcaPorId(long id)
        {
            var response = await _marcaService.ObtenerMarcaPorId(id);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> GuardarOActualizarMarca(Marca marca)
        {
            var tc = SessionHelperUser();
            if (marca.Id == 0)
            {
                marca.CreadoPor = tc ?? "system";
                marca.FechaCreacion = System.DateTime.Now;
            }
            else
            {
                marca.ModificadoPor = tc ?? "system";
                marca.FechaModificacion = System.DateTime.Now;
            }

            marca.Estatus = true;

            var response = await _marcaService.GuardarOActualizarMarca(marca);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> EliminarMarca(Marca marca)
        {
            var response = await _marcaService.EliminarMarca(marca);
            return JsonConvert.SerializeObject(response);
        }

        private string SessionHelperUser()
        {
            return PuntoDeVentaMVC.Helpers.SessionHelper.GetSessionUser()?.UserName;
        }

        #endregion
    }
}
