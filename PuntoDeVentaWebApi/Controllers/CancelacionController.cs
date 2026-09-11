using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaEntities.Ventas;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Cancelacion")]
    public class CancelacionController : BaseController
    {
        private readonly CancelacionService _cancelacionService;

        public CancelacionController()
        {
            _cancelacionService = new CancelacionService();
        }

        [HttpGet, Route("List")]
        public ModelResponse<List<CancelacionDTO>> ObtenerTodasLasCancelaciones(long sucursalId = 0)
        {
            return _cancelacionService.ObtenerCancelaciones(ObtenerEmpresaIdDesdeClaim(), sucursalId);
        }

        [HttpGet, Route("{id:long}")]
        public ModelResponse<CancelacionDTO> ObtenerCancelacionPorId(long id)
        {
            return _cancelacionService.ObtenerCancelacionPorId(ObtenerEmpresaIdDesdeClaim(), id);
        }

        [HttpPost, Route("Guardar")]
        public ModelResponse<CancelacionDTO> GuardarCancelacion(CancelacionDTO cancelacion)
        {
            return _cancelacionService.GuardarCancelacion(ObtenerEmpresaIdDesdeClaim(), cancelacion, User.Identity.Name);
        }

        // No se expone "Eliminar": una cancelación es un documento de auditoría inmutable
        // y no existe stored procedure de borrado lógico asociado (sp_Cancelacion_*).
    }
}
