using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Proveedor")]
    public class ProveedorController : BaseController
    {
        private readonly ProveedorService _proveedorService;

        public ProveedorController()
        {
            _proveedorService = new ProveedorService();
        }

        [HttpGet, Route("List")]
        public ModelResponse<List<Proveedor>> ObtenerTodosLosProveedores()
        {
            return _proveedorService.ObtenerProveedores(ObtenerEmpresaIdDesdeClaim());
        }

        [HttpGet, Route("{id:long}")]
        public ModelResponse<Proveedor> ObtenerProveedorPorId(long id)
        {
            return _proveedorService.ObtenerProveedorPorId(ObtenerEmpresaIdDesdeClaim(), id);
        }

        [HttpPost, Route("Guardar")]
        public ModelResponse<Proveedor> GuardarOActualizarProveedor(Proveedor proveedor)
        {
            return _proveedorService.GuardarOActualizarProveedor(ObtenerEmpresaIdDesdeClaim(), proveedor, User.Identity.Name);
        }

        [HttpDelete, Route("Eliminar")]
        public ModelResponse EliminarProveedor(Proveedor proveedor)
        {
            return _proveedorService.EliminarProveedor(ObtenerEmpresaIdDesdeClaim(), proveedor.Id, User.Identity.Name);
        }
    }
}
