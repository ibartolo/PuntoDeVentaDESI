using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Inventario;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    /// <summary>Servicio del front para inventario/stock (passthrough + unwrap).</summary>
    public class StockService
    {
        private readonly HttpClientConnection _httpClient;

        public StockService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ModelResponse<List<StockDTO>>> ConsultarStockPorSucursal(long sucursalId)
        {
            return await _httpClient.ObtenerStockPorSucursal(sucursalId);
        }

        public async Task<ModelResponse<StockDTO>> ConsultarStock(long sucursalId, long productoId)
        {
            return await _httpClient.ObtenerStock(sucursalId, productoId);
        }

        public async Task<ModelResponse<List<StockMovimientoDTO>>> ConsultarMovimientos(long sucursalId, long productoId)
        {
            return await _httpClient.ObtenerMovimientos(sucursalId, productoId);
        }

        public async Task<ModelResponse<List<ProductoDTO>>> ConsultarProductosParaStock()
        {
            return await _httpClient.ObtenerProductosParaStock();
        }

        public async Task<ModelResponse<decimal>> RegistrarMovimientoStock(StockMovimientoDTO movimiento)
        {
            return await _httpClient.RegistrarMovimientoStock(movimiento);
        }
    }
}
