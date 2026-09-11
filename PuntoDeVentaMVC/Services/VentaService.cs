using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaEntities.Ventas;
using PuntoDeVentaMVC.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    /// <summary>Servicio del front para ventas POS (passthrough + unwrap).</summary>
    public class VentaService
    {
        private readonly HttpClientConnection _httpClient;

        public VentaService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>Get-by-id: hace unwrap y devuelve la venta o null.</summary>
        public async Task<VentaDTO> ObtenerVentaPorId(long id)
        {
            var response = await _httpClient.ObtenerVentaPorId(id);
            if (response.IsSuccess && response.Response != null)
            {
                return response.Response;
            }

            return null;
        }

        public async Task<ModelResponse<VentaDTO>> GuardarVenta(VentaDTO venta)
        {
            return await _httpClient.GuardarVenta(venta);
        }

        public async Task<ModelResponse<List<VentaDTO>>> ConsultarTodasLasVentas(long sucursalId = 0)
        {
            return await _httpClient.ObtenerTodasLasVentas(sucursalId);
        }
    }
}
