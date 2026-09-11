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
        public async Task<ModelResponse<List<CategoriaDTO>>> ObtenerTodasLasCategorias()
        {
            return await RequestAsync<List<CategoriaDTO>>("api/Categoria/List", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<List<CategoriaDTO>>> ObtenerCategoriasPorPadre(long id)
        {
            return await RequestAsync<List<CategoriaDTO>>($"api/Categoria/PorPadre/{id}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<Categoria>> ObtenerCategoriaPorId(long id)
        {
            return await RequestAsync<Categoria>($"api/Categoria/{id}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<Categoria>> GuardarOActualizarCategoria(Categoria categoria)
        {
            MappingColumSecurity(categoria);
            return await RequestAsync<Categoria>("api/Categoria/Guardar", HttpMethod.Post, categoria, token?.Token?.access_token);
        }

        public async Task<ModelResponse> EliminarCategoria(Categoria categoria)
        {
            MappingColumSecurity(categoria);
            var result = await RequestAsync<object>("api/Categoria/Eliminar", HttpMethod.Delete, categoria,
                new Func<string, string>(responseString => responseString), token?.Token?.access_token);

            return JsonConvert.DeserializeObject<ModelResponse>(result.ToString());
        }
    }
}
