using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaEntities.Ventas;
using PuntoDeVentaMVC.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    /// <summary>Servicio del front para cancelaciones/devoluciones (passthrough + unwrap).</summary>
    public class CancelacionService
    {
        private readonly HttpClientConnection _httpClient;

        public CancelacionService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>Get-by-id: hace unwrap y devuelve la entidad o null.</summary>
        public async Task<CancelacionDTO> ObtenerCancelacionPorId(long id)
        {
            var response = await _httpClient.ObtenerCancelacionPorId(id);
            if (response.IsSuccess && response.Response != null)
            {
                return response.Response;
            }

            return null;
        }

        public async Task<ModelResponse<CancelacionDTO>> GuardarCancelacion(CancelacionDTO cancelacion)
        {
            return await _httpClient.GuardarCancelacion(cancelacion);
        }

        public async Task<ModelResponse<List<CancelacionDTO>>> ConsultarTodasLasCancelaciones(long sucursalId = 0)
        {
            return await _httpClient.ObtenerTodasLasCancelaciones(sucursalId);
        }
    }
}
