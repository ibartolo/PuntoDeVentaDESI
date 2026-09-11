using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PuntoDeVenta.MVC.Models;
using PuntoDeVenta.MVC.Services;

namespace PuntoDeVenta.MVC.Controllers
{
    [Authorize]
    public class MarcasController : Controller
    {
        private const string ServerBearerSessionKey = "ServerBearerToken";
        private readonly MarcasApiClient _marcasApiClient;

        public MarcasController()
        {
            _marcasApiClient = new MarcasApiClient();
        }

        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            var token = ObtenerBearerSesion();
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var response = await _marcasApiClient.ListarAsync(token.AccessToken);
            if (response == null)
            {
                ModelState.AddModelError(string.Empty, "No fue posible consultar Marcas.");
                return View(new List<MarcaViewModel>());
            }

            if (!response.Success)
            {
                CargarErrores(response.Message, response.Errors);
                return View(new List<MarcaViewModel>());
            }

            var activas = (response.Data ?? new List<MarcaViewModel>()).Where(x => x.Estatus).ToList();
            return View(activas);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new MarcaUpsertViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Create(MarcaUpsertViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var token = ObtenerBearerSesion();
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var response = await _marcasApiClient.CrearAsync(token.AccessToken, model);
            if (response == null || !response.Success)
            {
                CargarErrores(response?.Message ?? "No fue posible crear la Marca.", response?.Errors);
                return View(model);
            }

            TempData["SuccessMessage"] = "Marca creada correctamente.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> Edit(long id)
        {
            var consulta = await ConsultarMarcaActiva(id);
            if (consulta.Resultado != null)
            {
                return consulta.Resultado;
            }

            var marca = consulta.Marca;
            var model = new MarcaUpsertViewModel
            {
                Id = marca.Id,
                Nombre = marca.Nombre,
                Descripcion = marca.Descripcion
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Edit(MarcaUpsertViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var token = ObtenerBearerSesion();
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var response = await _marcasApiClient.ActualizarAsync(token.AccessToken, model);
            if (response == null || !response.Success)
            {
                CargarErrores(response?.Message ?? "No fue posible actualizar la Marca.", response?.Errors);
                return View(model);
            }

            TempData["SuccessMessage"] = "Marca actualizada correctamente.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> Details(long id)
        {
            var consulta = await ConsultarMarcaActiva(id);
            if (consulta.Resultado != null)
            {
                return consulta.Resultado;
            }

            return View(consulta.Marca);
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> Delete(long id)
        {
            var consulta = await ConsultarMarcaActiva(id);
            if (consulta.Resultado != null)
            {
                return consulta.Resultado;
            }

            return View(consulta.Marca);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async System.Threading.Tasks.Task<ActionResult> DeleteConfirmed(long id)
        {
            var token = ObtenerBearerSesion();
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var response = await _marcasApiClient.EliminarLogicoAsync(token.AccessToken, id);
            if (response == null || !response.Success)
            {
                CargarErrores(response?.Message ?? "No fue posible desactivar la Marca.", response?.Errors);

                var consulta = await ConsultarMarcaActiva(id, permitirInactivaSiError: true);
                if (consulta.Resultado != null)
                {
                    return consulta.Resultado;
                }

                return View("Delete", consulta.Marca);
            }

            TempData["SuccessMessage"] = "Marca desactivada correctamente.";
            return RedirectToAction("Index");
        }

        private ServerSessionToken ObtenerBearerSesion()
        {
            var token = Session?[ServerBearerSessionKey] as ServerSessionToken;
            if (token == null || token.ExpirationUtc <= System.DateTime.UtcNow || string.IsNullOrWhiteSpace(token.AccessToken))
            {
                Session?.Remove(ServerBearerSessionKey);
                return null;
            }

            return token;
        }

        private void CargarErrores(string message, IList<string> errors)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                ModelState.AddModelError(string.Empty, message);
            }

            if (errors == null)
            {
                return;
            }

            foreach (var error in errors.Where(e => !string.IsNullOrWhiteSpace(e)))
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        private async System.Threading.Tasks.Task<(MarcaViewModel Marca, ActionResult Resultado)> ConsultarMarcaActiva(long id, bool permitirInactivaSiError = false)
        {
            var token = ObtenerBearerSesion();
            if (token == null)
            {
                return (null, RedirectToAction("Login", "Account"));
            }

            var response = await _marcasApiClient.ConsultarAsync(token.AccessToken, id);
            if (response == null || !response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = response?.Message ?? "No fue posible consultar la Marca solicitada.";
                return (null, RedirectToAction("Index"));
            }

            if (!response.Data.Estatus && !permitirInactivaSiError)
            {
                TempData["ErrorMessage"] = "La Marca solicitada está inactiva y no forma parte del catálogo operativo.";
                return (null, RedirectToAction("Index"));
            }

            return (response.Data, null);
        }
    }
}
