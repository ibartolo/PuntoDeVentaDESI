using Newtonsoft.Json;
using PuntoDeVentaEntities.Autenticacion;
using PuntoDeVentaEntities.Seguridad;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.DAL
{
    public partial class HttpClientConnection
    {
        public async Task<ModelResponse<List<UsuarioDTO>>> ObtenerUsuarios()
        {
            return await RequestAsync<List<UsuarioDTO>>("api/Autenticacion/User/List", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<Usuario>> ObtenerUsuarioPorId(long id)
        {
            return await RequestAsync<Usuario>($"api/Autenticacion/User/{id}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<Usuario>> GuardarOActualizarUsuario(Usuario usuario)
        {
            return await RequestAsync<Usuario>("api/Autenticacion/User", HttpMethod.Post, usuario, token?.Token?.access_token);
        }

        public async Task<ModelResponse> EliminarUsuario(Usuario usuario)
        {
            var result = await RequestAsync<object>("api/Autenticacion/User", HttpMethod.Delete, usuario,
                new Func<string, string>(responseString => responseString), token?.Token?.access_token);

            return JsonConvert.DeserializeObject<ModelResponse>(result.ToString());
        }
    }
}
