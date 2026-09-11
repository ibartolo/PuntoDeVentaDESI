using PuntoDeVentaMVC.Services;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Controllers
{
    public class PermissionsController : BaseController
    {
        private readonly PermisosService _permisosService;

        public PermissionsController()
        {
            _permisosService = new PermisosService(httpClientConnection);
        }

        public async Task<string> ConsultarPermisosUsuario()
        {
            var response = await _permisosService.ObtenerPermisosPorUsuario();
            return Newtonsoft.Json.JsonConvert.SerializeObject(response);
        }

        public async Task<string> ValidarPermisoUsuario(string nombrePagina, string accion)
        {
            var response = await _permisosService.ValidarPermisoUsuario(nombrePagina, accion);
            return Newtonsoft.Json.JsonConvert.SerializeObject(response);
        }
    }
}
