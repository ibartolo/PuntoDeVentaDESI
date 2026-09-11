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
        public async Task<ModelResponse<List<Cliente>>> ObtenerTodosLosClientes()
        {
            return await RequestAsync<List<Cliente>>("api/Cliente/List", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<Cliente>> ObtenerClientePorId(long id)
        {
            return await RequestAsync<Cliente>($"api/Cliente/{id}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<Cliente>> GuardarOActualizarCliente(Cliente cliente)
        {
            MappingColumSecurity(cliente);
            return await RequestAsync<Cliente>("api/Cliente/Guardar", HttpMethod.Post, cliente, token?.Token?.access_token);
        }

        public async Task<ModelResponse> EliminarCliente(Cliente cliente)
        {
            MappingColumSecurity(cliente);
            var result = await RequestAsync<object>("api/Cliente/Eliminar", HttpMethod.Delete, cliente,
                new Func<string, string>(responseString => responseString), token?.Token?.access_token);

            return JsonConvert.DeserializeObject<ModelResponse>(result.ToString());
        }
    }
}
