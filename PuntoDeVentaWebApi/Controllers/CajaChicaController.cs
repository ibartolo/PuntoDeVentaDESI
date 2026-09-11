using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Caja;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    /// <summary>
    /// API de caja chica: apertura por usuario+sucursal, cierre y salidas (límite 50%).
    /// </summary>
    [Authorize]
    [RoutePrefix("api/CajaChica")]
    public class CajaChicaController : BaseController
    {
        private readonly CajaChicaService _cajaChicaService;

        public CajaChicaController()
        {
            _cajaChicaService = new CajaChicaService();
        }

        /// <summary>Obtiene la caja chica abierta de un usuario en una sucursal.</summary>
        [HttpGet, Route("Abierta")]
        public ModelResponse<CajaChicaDTO> ObtenerCajaChicaAbierta(long sucursalId, long usuarioId)
        {
            return _cajaChicaService.ObtenerCajaChicaAbierta(ObtenerEmpresaIdDesdeClaim(), sucursalId, usuarioId);
        }

        /// <summary>Abre una caja chica para el usuario y la sucursal indicados.</summary>
        [HttpPost, Route("Abrir")]
        public ModelResponse<CajaChicaDTO> AbrirCajaChica(CajaChicaDTO caja)
        {
            if (caja == null)
            {
                return new ModelResponse<CajaChicaDTO> { IsSuccess = false, Message = "La caja chica es requerida." };
            }

            return _cajaChicaService.AbrirCajaChica(ObtenerEmpresaIdDesdeClaim(), caja.SucursalId, caja.UsuarioId,
                caja.MontoInicial, User.Identity.Name);
        }

        /// <summary>Cierra la caja chica indicada.</summary>
        [HttpPost, Route("Cerrar")]
        public ModelResponse CerrarCajaChica(CajaChicaDTO caja)
        {
            if (caja == null)
            {
                return new ModelResponse { IsSuccess = false, Message = "La caja chica es requerida." };
            }

            return _cajaChicaService.CerrarCajaChica(ObtenerEmpresaIdDesdeClaim(), caja.Id, User.Identity.Name);
        }

        /// <summary>Lista las salidas registradas para una caja chica.</summary>
        [HttpGet, Route("Salidas/{cajaChicaId:long}")]
        public ModelResponse<List<SalidaCaja>> ObtenerSalidasCaja(long cajaChicaId)
        {
            return _cajaChicaService.ObtenerSalidasCaja(ObtenerEmpresaIdDesdeClaim(), cajaChicaId);
        }

        /// <summary>Registra una salida de caja (valida el límite del 50% de los ingresos).</summary>
        [HttpPost, Route("Salida")]
        public ModelResponse<SalidaCaja> RegistrarSalidaCaja(SalidaCaja salida)
        {
            return _cajaChicaService.RegistrarSalidaCaja(ObtenerEmpresaIdDesdeClaim(), salida, User.Identity.Name);
        }
    }
}
