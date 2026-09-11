using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaEntities.Ventas;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.DAL
{
    /// <summary>
    /// Acceso HTTP del front a la WebApi para ventas POS.
    /// Los endpoints espejan las rutas del VentaController de la WebApi (api/Venta).
    /// </summary>
    public partial class HttpClientConnection
    {
        /// <summary>Lista las ventas de la empresa; opcionalmente filtra por sucursal.</summary>
        public async Task<ModelResponse<List<VentaDTO>>> ObtenerTodasLasVentas(long sucursalId = 0)
        {
            return await RequestAsync<List<VentaDTO>>($"api/Venta/List?sucursalId={sucursalId}", HttpMethod.Get, null,
                token?.Token?.access_token);
        }

        /// <summary>Obtiene una venta con su detalle por Id.</summary>
        public async Task<ModelResponse<VentaDTO>> ObtenerVentaPorId(long id)
        {
            return await RequestAsync<VentaDTO>($"api/Venta/{id}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        /// <summary>Registra una venta (descuenta stock y registra el ingreso en caja en la WebApi).</summary>
        public async Task<ModelResponse<VentaDTO>> GuardarVenta(VentaDTO venta)
        {
            MappingColumSecurity(venta);
            return await RequestAsync<VentaDTO>("api/Venta/Guardar", HttpMethod.Post, venta, token?.Token?.access_token);
        }
    }
}
