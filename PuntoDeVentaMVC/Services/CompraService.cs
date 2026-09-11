using PuntoDeVentaEntities.Compras;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    /// <summary>Servicio del front para el historial de compras (passthrough + unwrap).</summary>
    public class CompraService
    {
        private readonly HttpClientConnection _httpClient;

        public CompraService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ModelResponse<List<CompraDTO>>> ConsultarTodasLasCompras(long sucursalId = 0)
        {
            return await _httpClient.ObtenerCompras(sucursalId);
        }

        /// <summary>Get-by-id: hace unwrap y devuelve la entidad o null.</summary>
        public async Task<CompraDTO> ObtenerCompraPorId(long id)
        {
            var response = await _httpClient.ObtenerCompraPorId(id);
            if (response.IsSuccess && response.Response != null)
            {
                return response.Response;
            }

            return null;
        }

        public async Task<ModelResponse<CompraDTO>> GuardarCompra(CompraDTO compra)
        {
            return await _httpClient.GuardarCompra(compra);
        }

        public async Task<ModelResponse> EliminarCompra(CompraDTO compra)
        {
            return await _httpClient.EliminarCompra(compra);
        }
    }
}
