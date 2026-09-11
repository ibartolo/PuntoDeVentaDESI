using Newtonsoft.Json;
using PuntoDeVentaEntities.Compras;
using PuntoDeVentaEntities.Seguridad;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.DAL
{
    public partial class HttpClientConnection
    {
        public async Task<ModelResponse<List<CompraDTO>>> ObtenerCompras(long sucursalId = 0)
        {
            return await RequestAsync<List<CompraDTO>>($"api/Compra/List?sucursalId={sucursalId}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<CompraDTO>> ObtenerCompraPorId(long id)
        {
            return await RequestAsync<CompraDTO>($"api/Compra/{id}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<CompraDTO>> GuardarCompra(CompraDTO compra)
        {
            MappingColumSecurity(compra);
            return await RequestAsync<CompraDTO>("api/Compra/Guardar", HttpMethod.Post, compra, token?.Token?.access_token);
        }

        public async Task<ModelResponse> EliminarCompra(CompraDTO compra)
        {
            MappingColumSecurity(compra);
            var result = await RequestAsync<object>("api/Compra/Eliminar", HttpMethod.Delete, compra,
                new Func<string, string>(responseString => responseString), token?.Token?.access_token);

            return JsonConvert.DeserializeObject<ModelResponse>(result.ToString());
        }
    }
}
