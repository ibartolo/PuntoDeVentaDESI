using Newtonsoft.Json;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.Filters;
using PuntoDeVentaMVC.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PuntoDeVentaMVC.Controllers
{
    public class SecurityController : BaseController
    {
        private readonly PermisosService _permisosService;
        private readonly RolService _rolService;

        public SecurityController()
        {
            _permisosService = new PermisosService(httpClientConnection);
            _rolService = new RolService(httpClientConnection);
        }

        #region Views

        public async Task<ActionResult> Role(long id = 0)
        {
            var permisos = await _permisosService.ObtenerPermisosParaPagina("Roles");
            if (permisos == null || !permisos.PuedeLeer)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            var rol = new Rol();
            if (id > 0)
            {
                var encontrado = await _rolService.ObtenerRolPorId(id);
                if (encontrado != null)
                {
                    rol = encontrado;
                }
                else
                {
                    ViewBag.ErrorMessage = "No se encontró el rol.";
                }
            }

            ViewBag.Permisos = permisos;
            return View(rol);
        }

        public async Task<ActionResult> Permisos()
        {
            var permisos = await _permisosService.ObtenerPermisosParaPagina("Permisos");
            if (permisos == null || !permisos.PuedeLeer)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            var rolesResponse = await _rolService.ObtenerTodosLosRoles();
            ViewBag.Roles = rolesResponse.IsSuccess && rolesResponse.Response != null
                ? rolesResponse.Response
                : new List<Rol>();

            var paginasResponse = await _permisosService.ObtenerPaginas();
            ViewBag.Paginas = paginasResponse.IsSuccess && paginasResponse.Response != null
                ? paginasResponse.Response
                : new List<Pagina>();

            ViewBag.Permisos = permisos;
            return View();
        }

        #endregion

        #region Data Access

        public async Task<string> ConsultarTodosLosRoles()
        {
            var response = await _rolService.ObtenerTodosLosRoles();
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarRolPorId(long id)
        {
            var response = await _rolService.ObtenerRolPorId(id);
            return JsonConvert.SerializeObject(response);
        }

        [Permiso("Roles")]
        public async Task<string> GuardarOActualizarRol(Rol r)
        {
            var response = await _rolService.GuardarOActualizarRol(r);
            return JsonConvert.SerializeObject(response);
        }

        [Permiso("Roles", "Eliminar")]
        public async Task<string> EliminarRol(Rol r)
        {
            var response = await _rolService.EliminarRol(r);
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ObtenerPermisosPorRol(long rolId)
        {
            var response = await _permisosService.ObtenerPermisosPorRol(rolId);
            return JsonConvert.SerializeObject(response);
        }

        [Permiso("Permisos", "Leer")]
        public async Task<string> ConsultarConteoPaginasPorRol()
        {
            var response = await _permisosService.ObtenerConteoPaginasPorRol();
            return JsonConvert.SerializeObject(response);
        }

        [Permiso("Permisos", "Editar")]
        public async Task<string> GuardarPermisosRol(GuardarPermisosRequest request)
        {
            var response = await _permisosService.GuardarPermisosRol(request);
            return JsonConvert.SerializeObject(response);
        }

        [Permiso("Permisos", "Editar")]
        public async Task<string> GuardarPermisosRolMasivo(GuardarPermisosMasivoRequest request)
        {
            var response = await _permisosService.GuardarPermisosRolMasivo(request);
            return JsonConvert.SerializeObject(response);
        }

        #endregion
    }
}
