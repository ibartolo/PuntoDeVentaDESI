using Newtonsoft.Json;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.DAL
{
    public partial class HttpClientConnection
    {
        public async Task<ModelResponse<PrecioDTO>> ObtenerPrecioActivo(long productoId)
        {
            return await RequestAsync<PrecioDTO>($"api/Precio/Activo/{productoId}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<List<PrecioDTO>>> ObtenerPreciosPorProducto(long productoId)
        {
            return await RequestAsync<List<PrecioDTO>>($"api/Precio/PorProducto/{productoId}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<PrecioDTO>> GuardarOActualizarPrecio(Precio precio)
        {
            MappingColumSecurity(precio);
            return await RequestAsync<PrecioDTO>("api/Precio/Guardar", HttpMethod.Post, precio, token?.Token?.access_token);
        }
    }
}
