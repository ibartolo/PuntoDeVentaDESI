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
        public async Task<ModelResponse<List<Proveedor>>> ObtenerTodosLosProveedores()
        {
            return await RequestAsync<List<Proveedor>>("api/Proveedor/List", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<Proveedor>> ObtenerProveedorPorId(long id)
        {
            return await RequestAsync<Proveedor>($"api/Proveedor/{id}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<Proveedor>> GuardarOActualizarProveedor(Proveedor proveedor)
        {
            MappingColumSecurity(proveedor);
            return await RequestAsync<Proveedor>("api/Proveedor/Guardar", HttpMethod.Post, proveedor, token?.Token?.access_token);
        }

        public async Task<ModelResponse> EliminarProveedor(Proveedor proveedor)
        {
            MappingColumSecurity(proveedor);
            var result = await RequestAsync<object>("api/Proveedor/Eliminar", HttpMethod.Delete, proveedor,
                new Func<string, string>(responseString => responseString), token?.Token?.access_token);

            return JsonConvert.DeserializeObject<ModelResponse>(result.ToString());
        }
    }
}
