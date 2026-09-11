using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaEntities.Ventas;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Venta")]
    public class VentaController : BaseController
    {
        private readonly VentaService _ventaService;

        public VentaController()
        {
            _ventaService = new VentaService();
        }

        [HttpGet, Route("List")]
        public ModelResponse<List<VentaDTO>> ObtenerVentas(long sucursalId = 0)
        {
            return _ventaService.ObtenerVentas(ObtenerEmpresaIdDesdeClaim(), sucursalId);
        }

        [HttpGet, Route("{id:long}")]
        public ModelResponse<VentaDTO> ObtenerVentaPorId(long id)
        {
            return _ventaService.ObtenerVentaPorId(ObtenerEmpresaIdDesdeClaim(), id);
        }

        [HttpPost, Route("Guardar")]
        public ModelResponse<VentaDTO> GuardarVenta(VentaDTO venta)
        {
            if (venta != null && venta.UsuarioId <= 0)
            {
                venta.UsuarioId = ObtenerUsuarioIdDesdeClaim();
            }

            return _ventaService.GuardarVenta(ObtenerEmpresaIdDesdeClaim(), venta, User.Identity.Name);
        }
    }
}
