using PuntoDeVentaEntities.Caja;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    /// <summary>Servicio del front para cortes de caja (passthrough + unwrap).</summary>
    public class CorteService
    {
        private readonly HttpClientConnection _httpClient;

        public CorteService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>Get-by-id: hace unwrap y devuelve la entidad o null.</summary>
        public async Task<CorteDTO> ObtenerCortePorId(long id)
        {
            var response = await _httpClient.ObtenerCortePorId(id);
            if (response.IsSuccess && response.Response != null)
            {
                return response.Response;
            }

            return null;
        }

        /// <summary>Consulta con la respuesta completa (Data Access / detalle).</summary>
        public async Task<ModelResponse<CorteDTO>> ConsultarCortePorId(long id)
        {
            return await _httpClient.ObtenerCortePorId(id);
        }

        public async Task<ModelResponse<CorteDTO>> GuardarCorte(CorteDTO corte)
        {
            return await _httpClient.GuardarCorte(corte);
        }

        public async Task<ModelResponse<List<CorteDTO>>> ConsultarTodosLosCortes(long sucursalId = 0)
        {
            return await _httpClient.ObtenerTodosLosCortes(sucursalId);
        }
    }
}
