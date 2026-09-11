using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Sucursal")]
    public class SucursalController : BaseController
    {
        private readonly SucursalService _sucursalService;

        public SucursalController()
        {
            _sucursalService = new SucursalService();
        }

        [HttpGet, Route("List")]
        public ModelResponse<List<Sucursal>> ObtenerTodasLasSucursales()
        {
            return _sucursalService.ObtenerSucursales(ObtenerEmpresaIdDesdeClaim());
        }

        [HttpGet, Route("{id:long}")]
        public ModelResponse<Sucursal> ObtenerSucursalPorId(long id)
        {
            return _sucursalService.ObtenerSucursalPorId(ObtenerEmpresaIdDesdeClaim(), id);
        }

        [HttpPost, Route("Guardar")]
        public ModelResponse<Sucursal> GuardarOActualizarSucursal(Sucursal sucursal)
        {
            return _sucursalService.GuardarOActualizarSucursal(ObtenerEmpresaIdDesdeClaim(), sucursal, User.Identity.Name);
        }

        [HttpDelete, Route("Eliminar")]
        public ModelResponse EliminarSucursal(Sucursal sucursal)
        {
            return _sucursalService.EliminarSucursal(ObtenerEmpresaIdDesdeClaim(), sucursal.Id, User.Identity.Name);
        }
    }
}
