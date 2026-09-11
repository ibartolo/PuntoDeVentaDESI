using Newtonsoft.Json;
using PuntoDeVentaEntities.Autenticacion;
using PuntoDeVentaEntities.Seguridad;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.DAL
{
    public partial class HttpClientConnection
    {
        /// <summary>Paso 1 del login: valida credenciales (sin token) y devuelve el usuario.</summary>
        public async Task<ModelResponse<Usuario>> AutenticarUsuario(Usuario usuario)
        {
            var result = await RequestAsync<Usuario>("api/Autenticacion/autenticar", HttpMethod.Post, usuario);
            Log.Information("AutenticarUsuario (MVC DAL) para {Usuario}: IsSuccess={IsSuccess}, Message={Message}", usuario.NombreUsuario, result?.IsSuccess, result?.Message);
            return result;
        }

        /// <summary>Solicita el envío del correo de recuperación (sin token).</summary>
        public async Task<ModelResponse> SolicitarRecuperacion(string nombreUsuarioOCorreo)
        {
            var request = new
            {
                NombreUsuario = nombreUsuarioOCorreo
            };

            var result = await RequestAsync<object>("api/Autenticacion/solicitarRecuperacion", HttpMethod.Post, request,
                new Func<string, string>(responseString => responseString));

            return JsonConvert.DeserializeObject<ModelResponse>(result.ToString());
        }

        /// <summary>Valida un token de recuperación de contraseña (sin token).</summary>
        public async Task<ModelResponse> ValidarTokenRecuperacion(string tokenRecuperacion)
        {
            var result = await RequestAsync<object>($"api/Autenticacion/validarToken/{tokenRecuperacion}", HttpMethod.Get, null,
                new Func<string, string>(responseString => responseString));

            return JsonConvert.DeserializeObject<ModelResponse>(result.ToString());
        }

        /// <summary>Restablece la contraseña a partir de un token de recuperación (sin token).</summary>
        public async Task<ModelResponse> RestablecerContrasenia(string tokenRecuperacion, string nuevaContrasena)
        {
            var request = new
            {
                Token = tokenRecuperacion,
                NuevaContrasena = nuevaContrasena
            };

            var result = await RequestAsync<object>("api/Autenticacion/restablecerContrasenia", HttpMethod.Post, request,
                new Func<string, string>(responseString => responseString));

            return JsonConvert.DeserializeObject<ModelResponse>(result.ToString());
        }
    }
}
