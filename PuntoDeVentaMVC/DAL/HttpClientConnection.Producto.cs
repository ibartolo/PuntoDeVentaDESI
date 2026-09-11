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
        public async Task<ModelResponse<List<ProductoDTO>>> ObtenerTodosLosProductos()
        {
            return await RequestAsync<List<ProductoDTO>>("api/Producto/List", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<ProductoDTO>> ObtenerProductoPorId(long id)
        {
            return await RequestAsync<ProductoDTO>($"api/Producto/{id}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<ProductoDTO>> ObtenerProductoPorCodigo(string codigo)
        {
            return await RequestAsync<ProductoDTO>($"api/Producto/PorCodigo/{codigo}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<ProductoDTO>> GuardarOActualizarProducto(Producto producto)
        {
            MappingColumSecurity(producto);
            return await RequestAsync<ProductoDTO>("api/Producto/Guardar", HttpMethod.Post, producto, token?.Token?.access_token);
        }

        public async Task<ModelResponse> EliminarProducto(Producto producto)
        {
            MappingColumSecurity(producto);
            var result = await RequestAsync<object>("api/Producto/Eliminar", HttpMethod.Delete, producto,
                new Func<string, string>(responseString => responseString), token?.Token?.access_token);

            return JsonConvert.DeserializeObject<ModelResponse>(result.ToString());
        }
    }
}
