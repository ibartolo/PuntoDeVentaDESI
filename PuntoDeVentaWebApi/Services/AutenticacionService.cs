using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Hosting;
using PuntoDeVentaEntities.Autenticacion;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using PuntoDeVentaWebApi.Helpers;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    public class AutenticacionService
    {
        private readonly DbWrapper _dbWrapper;

        public AutenticacionService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<Usuario> AutenticarUsuario(Usuario usuario)
        {
            try
            {
                Log.Information("AutenticacionService.AutenticarUsuario {NombreUsuario}", usuario?.NombreUsuario);
                if (usuario == null || string.IsNullOrWhiteSpace(usuario.NombreUsuario) || string.IsNullOrEmpty(usuario.Contrasena))
                {
                    throw new ArgumentException("Usuario y contraseña son requeridos.");
                }

                return _dbWrapper.AutenticarUsuario(usuario.NombreUsuario, usuario.Contrasena);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en AutenticarUsuario");
                return new ModelResponse<Usuario> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en AutenticarUsuario");
                return new ModelResponse<Usuario> { IsSuccess = false, Message = "Ocurrió un error al autenticar el usuario." };
            }
        }

        public ModelResponse<List<UsuarioDTO>> ObtenerUsuarios(long empresaId)
        {
            try
            {
                Log.Information("AutenticacionService.ObtenerUsuarios empresa {EmpresaId}", empresaId);
                if (empresaId <= 0) { throw new ArgumentException("No se pudo resolver la empresa autenticada."); }

                return _dbWrapper.ObtenerUsuarios(empresaId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerUsuarios");
                return new ModelResponse<List<UsuarioDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerUsuarios");
                return new ModelResponse<List<UsuarioDTO>> { IsSuccess = false, Message = "Ocurrió un error al obtener los usuarios." };
            }
        }

        public ModelResponse<Usuario> ObtenerUsuarioPorId(long empresaId, long usuarioId)
        {
            try
            {
                Log.Information("AutenticacionService.ObtenerUsuarioPorId {UsuarioId}", usuarioId);
                if (usuarioId <= 0) { throw new ArgumentException("El ID del usuario es requerido."); }

                return _dbWrapper.ObtenerUsuarioPorId(empresaId, usuarioId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerUsuarioPorId");
                return new ModelResponse<Usuario> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerUsuarioPorId");
                return new ModelResponse<Usuario> { IsSuccess = false, Message = "Ocurrió un error al obtener el usuario." };
            }
        }

        public ModelResponse<Usuario> GuardarOActualizarUsuario(long empresaId, Usuario usuario, string actor)
        {
            try
            {
                Log.Information("AutenticacionService.GuardarOActualizarUsuario empresa {EmpresaId}", empresaId);
                if (empresaId <= 0) { throw new ArgumentException("No se pudo resolver la empresa autenticada."); }
                if (usuario == null || string.IsNullOrWhiteSpace(usuario.NombreUsuario)) { throw new ArgumentException("El nombre de usuario es requerido."); }
                if (usuario.Id == 0 && string.IsNullOrWhiteSpace(usuario.Contrasena)) { throw new ArgumentException("La contraseña es requerida para un usuario nuevo."); }

                return _dbWrapper.GuardarOActualizarUsuario(empresaId, usuario, actor);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarOActualizarUsuario");
                return new ModelResponse<Usuario> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarOActualizarUsuario");
                return new ModelResponse<Usuario> { IsSuccess = false, Message = "Ocurrió un error al guardar el usuario." };
            }
        }

        public ModelResponse EliminarUsuario(long empresaId, long usuarioId, string actor)
        {
            try
            {
                Log.Information("AutenticacionService.EliminarUsuario {UsuarioId}", usuarioId);
                if (usuarioId <= 0) { throw new ArgumentException("El ID del usuario es requerido."); }

                return _dbWrapper.EliminarUsuario(empresaId, usuarioId, actor);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en EliminarUsuario");
                return new ModelResponse { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en EliminarUsuario");
                return new ModelResponse { IsSuccess = false, Message = "Ocurrió un error al desactivar el usuario." };
            }
        }

        public ModelResponse SolicitarRecuperacion(string usuarioOCorreo)
        {
            try
            {
                Log.Information("AutenticacionService.SolicitarRecuperacion {Valor}", usuarioOCorreo);

                if (string.IsNullOrWhiteSpace(usuarioOCorreo))
                {
                    throw new ArgumentException("Ingrese su usuario o correo.");
                }

                var usuarioResponse = _dbWrapper.ObtenerUsuarioPorNombreUsuario(usuarioOCorreo.Trim());
                if (!usuarioResponse.IsSuccess || usuarioResponse.Response == null)
                {
                    // No se revela si el usuario existe.
                    return new ModelResponse
                    {
                        IsSuccess = true,
                        Message = "Si el usuario existe, se envió un correo con instrucciones para restablecer la contraseña."
                    };
                }

                var usuario = usuarioResponse.Response;
                var token = Guid.NewGuid().ToString("N");
                var expiracion = DateTime.Now.AddHours(2);

                var crear = _dbWrapper.CrearTokenRecuperacion(usuario.Id, token, expiracion, "system");
                if (!crear.IsSuccess)
                {
                    return crear;
                }

                try
                {
                    var baseMvc = ConfigurationManager.AppSettings["BaseUriMVC"] ?? "http://localhost:5101/";
                    var enlace = baseMvc.TrimEnd('/') + "/Home/RecoverPassword/" + token;
                    var cuerpo = CargarPlantillaRecuperacion(usuario.NombreUsuario, token, enlace);

                    if (!string.IsNullOrWhiteSpace(usuario.Correo))
                    {
                        EmailHelper.EnvioEmail(new[] { usuario.Correo }, "Recuperación de contraseña", cuerpo, true);
                    }
                }
                catch (Exception exMail)
                {
                    Log.Error(exMail, "No se pudo enviar el correo de recuperación para {NombreUsuario}", usuario.NombreUsuario);
                }

                return new ModelResponse
                {
                    IsSuccess = true,
                    Message = "Se envió un correo con instrucciones para restablecer su contraseña."
                };
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en SolicitarRecuperacion");
                return new ModelResponse { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en SolicitarRecuperacion");
                return new ModelResponse { IsSuccess = false, Message = "Ocurrió un error al procesar la solicitud de recuperación." };
            }
        }

        public ModelResponse<TokenRecuperacionDTO> ValidarTokenRecuperacion(string token)
        {
            try
            {
                Log.Information("AutenticacionService.ValidarTokenRecuperacion");
                if (string.IsNullOrWhiteSpace(token)) { throw new ArgumentException("El token es requerido."); }

                return _dbWrapper.ObtenerTokenRecuperacion(token);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ValidarTokenRecuperacion");
                return new ModelResponse<TokenRecuperacionDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ValidarTokenRecuperacion");
                return new ModelResponse<TokenRecuperacionDTO> { IsSuccess = false, Message = "Ocurrió un error al validar el token." };
            }
        }

        public ModelResponse RestablecerContrasenia(string token, string nuevaContrasena)
        {
            try
            {
                Log.Information("AutenticacionService.RestablecerContrasenia");
                if (string.IsNullOrWhiteSpace(token)) { throw new ArgumentException("El token es requerido."); }
                if (string.IsNullOrWhiteSpace(nuevaContrasena)) { throw new ArgumentException("La nueva contraseña es requerida."); }

                return _dbWrapper.RestablecerContrasenia(token, nuevaContrasena, "system");
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en RestablecerContrasenia");
                return new ModelResponse { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en RestablecerContrasenia");
                return new ModelResponse { IsSuccess = false, Message = "Ocurrió un error al restablecer la contraseña." };
            }
        }

        private static string CargarPlantillaRecuperacion(string nombreUsuario, string token, string enlace)
        {
            var plantilla = null as string;

            try
            {
                var ruta = HostingEnvironment.MapPath("~/Template/Template_RecuperarEmail.html");
                if (!string.IsNullOrWhiteSpace(ruta) && File.Exists(ruta))
                {
                    plantilla = File.ReadAllText(ruta);
                }
            }
            catch
            {
                plantilla = null;
            }

            if (string.IsNullOrWhiteSpace(plantilla))
            {
                plantilla = "<p>Hola {{Usuario}},</p><p>Recibimos una solicitud para restablecer tu contraseña. Usa el siguiente enlace:</p><p><a href=\"{{Enlace}}\">{{Enlace}}</a></p><p>Código: {{Token}}</p>";
            }

            return plantilla
                .Replace("{{Usuario}}", nombreUsuario)
                .Replace("{{Token}}", token)
                .Replace("{{Enlace}}", enlace);
        }
    }
}
