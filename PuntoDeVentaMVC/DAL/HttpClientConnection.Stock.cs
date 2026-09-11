using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Inventario;
using PuntoDeVentaEntities.Seguridad;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.DAL
{
    public partial class HttpClientConnection
    {
        public async Task<ModelResponse<List<StockDTO>>> ObtenerStockPorSucursal(long sucursalId)
        {
            return await RequestAsync<List<StockDTO>>($"api/Stock/Sucursal/{sucursalId}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<StockDTO>> ObtenerStock(long sucursalId, long productoId)
        {
            return await RequestAsync<StockDTO>($"api/Stock/Sucursal/{sucursalId}/Producto/{productoId}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<List<StockMovimientoDTO>>> ObtenerMovimientos(long sucursalId = 0, long productoId = 0)
        {
            return await RequestAsync<List<StockMovimientoDTO>>($"api/Stock/Movimientos?sucursalId={sucursalId}&productoId={productoId}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<decimal>> RegistrarMovimientoStock(StockMovimientoDTO movimiento)
        {
            MappingColumSecurity(movimiento);
            return await RequestAsync<decimal>("api/Stock/Movimiento", HttpMethod.Post, movimiento, token?.Token?.access_token);
        }

        public async Task<ModelResponse<List<ProductoDTO>>> ObtenerProductosParaStock()
        {
            return await RequestAsync<List<ProductoDTO>>("api/Producto/List", HttpMethod.Get, null, token?.Token?.access_token);
        }
    }
}
