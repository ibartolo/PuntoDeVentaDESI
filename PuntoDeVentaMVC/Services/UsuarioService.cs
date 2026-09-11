using PuntoDeVentaEntities.Autenticacion;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    public class UsuarioService
    {
        private readonly HttpClientConnection _httpClient;

        public UsuarioService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Usuario> ObtenerUsuarioPorId(long id)
        {
            var response = await _httpClient.ObtenerUsuarioPorId(id);
            if (response.IsSuccess && response.Response != null)
            {
                return response.Response;
            }

            return null;
        }

        public async Task<ModelResponse<List<UsuarioDTO>>> ObtenerUsuarios()
        {
            return await _httpClient.ObtenerUsuarios();
        }

        public async Task<ModelResponse<Usuario>> GuardarOActualizarUsuarioAdmin(Usuario usuario)
        {
            return await _httpClient.GuardarOActualizarUsuario(usuario);
        }

        public async Task<ModelResponse> EliminarUsuario(Usuario usuario)
        {
            return await _httpClient.EliminarUsuario(usuario);
        }
    }
}
