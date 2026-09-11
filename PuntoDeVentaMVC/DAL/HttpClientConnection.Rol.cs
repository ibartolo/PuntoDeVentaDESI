using Newtonsoft.Json;
using PuntoDeVentaEntities.Seguridad;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.DAL
{
    public partial class HttpClientConnection
    {
        public async Task<ModelResponse<List<Rol>>> ObtenerTodosLosRoles()
        {
            return await RequestAsync<List<Rol>>("api/Rol/List", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<Rol>> ObtenerRolPorId(long id)
        {
            return await RequestAsync<Rol>($"api/Rol/{id}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<Rol>> GuardarOActualizarRol(Rol rol)
        {
            MappingColumSecurity(rol);
            return await RequestAsync<Rol>("api/Rol/Guardar", HttpMethod.Post, rol, token?.Token?.access_token);
        }

        public async Task<ModelResponse> EliminarRol(Rol rol)
        {
            MappingColumSecurity(rol);
            var result = await RequestAsync<object>("api/Rol/Eliminar", HttpMethod.Delete, rol,
                new Func<string, string>(responseString => responseString), token?.Token?.access_token);

            return JsonConvert.DeserializeObject<ModelResponse>(result.ToString());
        }

        public async Task<ModelResponse> AsignarRolUsuario(long usuarioId, long rolId)
        {
            var request = new
            {
                UsuarioId = usuarioId,
                RolId = rolId
            };

            var result = await RequestAsync<object>("api/Rol/Asignar", HttpMethod.Post, request,
                new Func<string, string>(responseString => responseString), token?.Token?.access_token);

            return JsonConvert.DeserializeObject<ModelResponse>(result.ToString());
        }

        public async Task<ModelResponse<List<Rol>>> ObtenerRolesPorUsuario(long usuarioId)
        {
            return await RequestAsync<List<Rol>>($"api/Rol/Usuario/{usuarioId}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<List<UsuarioRol>>> ObtenerUsuarioRolesPorUsuario(long usuarioId)
        {
            return await RequestAsync<List<UsuarioRol>>($"api/Rol/UsuarioRoles/{usuarioId}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse> EliminarRolUsuario(long usuarioRolId)
        {
            var request = new
            {
                UsuarioRolId = usuarioRolId
            };

            var result = await RequestAsync<object>("api/Rol/EliminarUsuarioRol", HttpMethod.Delete, request,
                new Func<string, string>(responseString => responseString), token?.Token?.access_token);

            return JsonConvert.DeserializeObject<ModelResponse>(result.ToString());
        }
    }
}
