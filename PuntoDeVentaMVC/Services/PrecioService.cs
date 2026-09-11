using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    /// <summary>Servicio del front para el historial de precios por producto (passthrough).</summary>
    public class PrecioService
    {
        private readonly HttpClientConnection _httpClient;

        public PrecioService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ModelResponse<PrecioDTO>> ObtenerPrecioActivo(long productoId)
        {
            return await _httpClient.ObtenerPrecioActivo(productoId);
        }

        public async Task<ModelResponse<List<PrecioDTO>>> ConsultarPreciosPorProducto(long productoId)
        {
            return await _httpClient.ObtenerPreciosPorProducto(productoId);
        }

        public async Task<ModelResponse<PrecioDTO>> GuardarOActualizarPrecio(Precio precio)
        {
            return await _httpClient.GuardarOActualizarPrecio(precio);
        }
    }
}
