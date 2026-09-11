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
        public async Task<ModelResponse<List<Marca>>> ObtenerTodasLasMarcas()
        {
            return await RequestAsync<List<Marca>>("api/Marca/List", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<Marca>> ObtenerMarcaPorId(long id)
        {
            return await RequestAsync<Marca>($"api/Marca/{id}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<Marca>> GuardarOActualizarMarca(Marca marca)
        {
            MappingColumSecurity(marca);
            return await RequestAsync<Marca>("api/Marca/Guardar", HttpMethod.Post, marca, token?.Token?.access_token);
        }

        public async Task<ModelResponse> EliminarMarca(Marca marca)
        {
            MappingColumSecurity(marca);
            var result = await RequestAsync<object>("api/Marca/Eliminar", HttpMethod.Delete, marca,
                new Func<string, string>(responseString => responseString), token?.Token?.access_token);

            return JsonConvert.DeserializeObject<ModelResponse>(result.ToString());
        }
    }
}
