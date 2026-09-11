using PuntoDeVentaEntities.Autenticacion;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.DAL;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    /// <summary>Servicio del front para el flujo de autenticación (passthrough al DAL HTTP).</summary>
    public class AutenticacionService
    {
        private readonly HttpClientConnection _httpClient;

        public AutenticacionService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ModelResponse<Usuario>> AutenticarUsuario(Usuario usuario)
        {
            return await _httpClient.AutenticarUsuario(usuario);
        }

        public async Task<ModelResponse> SolicitarRecuperacion(string nombreUsuarioOCorreo)
        {
            return await _httpClient.SolicitarRecuperacion(nombreUsuarioOCorreo);
        }

        public async Task<ModelResponse> ValidarTokenRecuperacion(string token)
        {
            return await _httpClient.ValidarTokenRecuperacion(token);
        }

        public async Task<ModelResponse> RestablecerContrasenia(string token, string nuevaContrasena)
        {
            return await _httpClient.RestablecerContrasenia(token, nuevaContrasena);
        }
    }
}
