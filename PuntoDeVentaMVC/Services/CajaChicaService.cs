using PuntoDeVentaEntities.Caja;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    /// <summary>Servicio del front para caja chica (passthrough + unwrap).</summary>
    public class CajaChicaService
    {
        private readonly HttpClientConnection _httpClient;

        public CajaChicaService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>Get caja abierta: hace unwrap y devuelve la entidad o null.</summary>
        public async Task<CajaChicaDTO> ObtenerCajaChicaAbierta(long sucursalId, long usuarioId)
        {
            var response = await _httpClient.ObtenerCajaChicaAbierta(sucursalId, usuarioId);
            if (response.IsSuccess && response.Response != null)
            {
                return response.Response;
            }

            return null;
        }

        /// <summary>Get caja abierta sin unwrap (para respuestas AJAX).</summary>
        public async Task<ModelResponse<CajaChicaDTO>> ConsultarCajaChicaAbierta(long sucursalId, long usuarioId)
        {
            return await _httpClient.ObtenerCajaChicaAbierta(sucursalId, usuarioId);
        }

        public async Task<ModelResponse<CajaChicaDTO>> AbrirCajaChica(CajaChicaDTO caja)
        {
            return await _httpClient.AbrirCajaChica(caja);
        }

        public async Task<ModelResponse> CerrarCajaChica(CajaChicaDTO caja)
        {
            return await _httpClient.CerrarCajaChica(caja);
        }

        public async Task<ModelResponse<List<SalidaCaja>>> ConsultarSalidasCaja(long cajaChicaId)
        {
            return await _httpClient.ObtenerSalidasCaja(cajaChicaId);
        }

        public async Task<ModelResponse<SalidaCaja>> RegistrarSalidaCaja(SalidaCaja salida)
        {
            return await _httpClient.RegistrarSalidaCaja(salida);
        }
    }
}
