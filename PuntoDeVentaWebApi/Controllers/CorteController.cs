using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Caja;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Corte")]
    public class CorteController : BaseController
    {
        private readonly CorteService _corteService;

        public CorteController()
        {
            _corteService = new CorteService();
        }

        [HttpGet, Route("List")]
        public ModelResponse<List<CorteDTO>> ObtenerCortes(long sucursalId = 0)
        {
            return _corteService.ObtenerCortes(ObtenerEmpresaIdDesdeClaim(), sucursalId);
        }

        [HttpGet, Route("{id:long}")]
        public ModelResponse<CorteDTO> ObtenerCortePorId(long id)
        {
            return _corteService.ObtenerCortePorId(ObtenerEmpresaIdDesdeClaim(), id);
        }

        [HttpPost, Route("Guardar")]
        public ModelResponse<CorteDTO> GuardarCorte(CorteDTO corte)
        {
            if (corte != null && corte.UsuarioId <= 0)
            {
                corte.UsuarioId = ObtenerUsuarioIdDesdeClaim();
            }

            return _corteService.GuardarCorte(ObtenerEmpresaIdDesdeClaim(), corte, User.Identity.Name);
        }
    }
}
