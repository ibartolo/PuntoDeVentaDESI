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
        public async Task<ModelResponse<List<Sucursal>>> ObtenerTodasLasSucursales()
        {
            return await RequestAsync<List<Sucursal>>("api/Sucursal/List", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<Sucursal>> ObtenerSucursalPorId(long id)
        {
            return await RequestAsync<Sucursal>($"api/Sucursal/{id}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<Sucursal>> GuardarOActualizarSucursal(Sucursal sucursal)
        {
            MappingColumSecurity(sucursal);
            return await RequestAsync<Sucursal>("api/Sucursal/Guardar", HttpMethod.Post, sucursal, token?.Token?.access_token);
        }

        public async Task<ModelResponse> EliminarSucursal(Sucursal sucursal)
        {
            MappingColumSecurity(sucursal);
            var result = await RequestAsync<object>("api/Sucursal/Eliminar", HttpMethod.Delete, sucursal,
                new Func<string, string>(responseString => responseString), token?.Token?.access_token);

            return JsonConvert.DeserializeObject<ModelResponse>(result.ToString());
        }
    }
}
