using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using PuntoDeVentaEntities.Autenticacion;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Helpers;
using Serilog;

namespace PuntoDeVentaWebApi.DAL
{
    public partial class DbWrapper
    {
        public ModelResponse<Usuario> ObtenerUsuarioPorNombreUsuario(string nombreUsuario)
        {
            var mr = new ModelResponse<Usuario>();
            try
            {
                var usuario = GetObject(
                    "sp_Usuario_ConsultarPorNombreUsuario",
                    new Func<IDataReader, Usuario>(r => LlenarEntidad<Usuario>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@NombreUsuario", nombreUsuario) });

                if (usuario == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Usuario no encontrado.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = usuario;
                mr.Message = "Usuario obtenido correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener usuario por nombre {NombreUsuario}", nombreUsuario);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener el usuario.";
            }

            return mr;
        }

        public ModelResponse<List<UsuarioDTO>> ObtenerUsuarios(long empresaId)
        {
            var mr = new ModelResponse<List<UsuarioDTO>>();
            try
            {
                var usuarios = GetObjects(
                    "sp_Usuario_Listar",
                    new Func<IDataReader, UsuarioDTO>(r => LlenarEntidad<UsuarioDTO>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId) });

                mr.IsSuccess = true;
                mr.Response = usuarios.ToList();
                mr.Message = "Usuarios obtenidos correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener usuarios de empresa {EmpresaId}", empresaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener los usuarios.";
            }

            return mr;
        }

        public ModelResponse<Usuario> ObtenerUsuarioPorId(long empresaId, long usuarioId)
        {
            var mr = new ModelResponse<Usuario>();
            try
            {
                var usuario = GetObject(
                    "sp_Usuario_Obtener",
                    new Func<IDataReader, Usuario>(r => LlenarEntidad<Usuario>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId), new SqlParameter("@UsuarioId", usuarioId) });

                if (usuario == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Usuario no encontrado.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = usuario;
                mr.Message = "Usuario obtenido correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener usuario {UsuarioId}", usuarioId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener el usuario.";
            }

            return mr;
        }

        public ModelResponse<Usuario> GuardarOActualizarUsuario(long empresaId, Usuario usuario, string actor)
        {
            var mr = new ModelResponse<Usuario>();
            try
            {
                if (usuario == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "El usuario es requerido.";
                    return mr;
                }

                var actorSeguro = (actor ?? usuario.CreadoPor ?? usuario.ModificadoPor ?? "system").Trim();
                if (actorSeguro.Length > 25)
                {
                    actorSeguro = actorSeguro.Substring(0, 25);
                }

                if (!string.IsNullOrWhiteSpace(usuario.Contrasena))
                {
                    var combined = Cryptography.HashPassword(usuario.Contrasena);
                    var parts = combined.Split('$');
                    usuario.ContrasenaIteraciones = int.Parse(parts[1]);
                    usuario.ContrasenaSalt = parts[2];
                    usuario.ContrasenaHash = parts[3];
                }

                var pars = new[]
                {
                    new SqlParameter("@Id", usuario.Id),
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@NombreUsuario", usuario.NombreUsuario),
                    new SqlParameter("@ContrasenaHash", (object)usuario.ContrasenaHash ?? DBNull.Value),
                    new SqlParameter("@ContrasenaSalt", (object)usuario.ContrasenaSalt ?? DBNull.Value),
                    new SqlParameter("@ContrasenaIteraciones", usuario.ContrasenaIteraciones > 0 ? (object)usuario.ContrasenaIteraciones : DBNull.Value),
                    new SqlParameter("@Correo", (object)usuario.Correo ?? DBNull.Value),
                    new SqlParameter("@Telefono", (object)usuario.Telefono ?? DBNull.Value),
                    new SqlParameter("@ImagenPerfil", (object)usuario.ImagenPerfil ?? DBNull.Value),
                    new SqlParameter("@Actor", actorSeguro)
                };

                var resultado = ExecuteScalar("sp_Usuario_Guardar", CommandType.StoredProcedure, pars);
                var resultadoLong = Convert.ToInt64(resultado);

                if (resultadoLong == -1)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Ya existe un usuario con ese nombre.";
                    return mr;
                }

                if (resultadoLong == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Datos incompletos: la contraseña es requerida para un usuario nuevo.";
                    return mr;
                }

                return ObtenerUsuarioPorId(empresaId, resultadoLong);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar usuario");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar el usuario.";
            }

            return mr;
        }

        public ModelResponse EliminarUsuario(long empresaId, long usuarioId, string actor)
        {
            var mr = new ModelResponse();
            try
            {
                var actorSeguro = (actor ?? "system").Trim();
                if (actorSeguro.Length > 25)
                {
                    actorSeguro = actorSeguro.Substring(0, 25);
                }

                var affected = ExecuteScalar("sp_Usuario_EliminarLogico", CommandType.StoredProcedure, new[]
                {
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@UsuarioId", usuarioId),
                    new SqlParameter("@Actor", actorSeguro)
                });

                if (affected != null && Convert.ToInt32(affected) == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Usuario no encontrado o ya inactivo.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Message = "Usuario desactivado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar usuario {UsuarioId}", usuarioId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al desactivar el usuario.";
            }

            return mr;
        }

        public ModelResponse CrearTokenRecuperacion(long usuarioId, string token, DateTime fechaExpiracion, string actor)
        {
            var mr = new ModelResponse();
            try
            {
                var actorSeguro = (actor ?? "system").Trim();
                if (actorSeguro.Length > 25)
                {
                    actorSeguro = actorSeguro.Substring(0, 25);
                }

                ExecuteScalar("sp_TokenRecuperacion_Crear", CommandType.StoredProcedure, new[]
                {
                    new SqlParameter("@UsuarioId", usuarioId),
                    new SqlParameter("@Token", token),
                    new SqlParameter("@FechaExpiracion", fechaExpiracion),
                    new SqlParameter("@Actor", actorSeguro)
                });

                mr.IsSuccess = true;
                mr.Message = "Token de recuperación creado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al crear token de recuperación para usuario {UsuarioId}", usuarioId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al crear el token de recuperación.";
            }

            return mr;
        }

        public ModelResponse<TokenRecuperacionDTO> ObtenerTokenRecuperacion(string token)
        {
            var mr = new ModelResponse<TokenRecuperacionDTO>();
            try
            {
                var resultado = GetObject(
                    "sp_TokenRecuperacion_ObtenerPorToken",
                    new Func<IDataReader, TokenRecuperacionDTO>(r => LlenarEntidad<TokenRecuperacionDTO>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@Token", token) });

                if (resultado == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "El enlace de recuperación no es válido o ha expirado.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = resultado;
                mr.Message = "Token válido";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener token de recuperación");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al validar el token de recuperación.";
            }

            return mr;
        }

        public ModelResponse RestablecerContrasenia(string token, string nuevaContrasena, string actor)
        {
            var mr = new ModelResponse();
            try
            {
                if (string.IsNullOrWhiteSpace(nuevaContrasena))
                {
                    mr.IsSuccess = false;
                    mr.Message = "La nueva contraseña es requerida.";
                    return mr;
                }

                var combined = Cryptography.HashPassword(nuevaContrasena);
                var parts = combined.Split('$');

                var actorSeguro = (actor ?? "system").Trim();
                if (actorSeguro.Length > 25)
                {
                    actorSeguro = actorSeguro.Substring(0, 25);
                }

                var resultado = ExecuteScalar("sp_TokenRecuperacion_RestablecerContrasenia", CommandType.StoredProcedure, new[]
                {
                    new SqlParameter("@Token", token),
                    new SqlParameter("@ContrasenaHash", parts[3]),
                    new SqlParameter("@ContrasenaSalt", parts[2]),
                    new SqlParameter("@ContrasenaIteraciones", int.Parse(parts[1])),
                    new SqlParameter("@Actor", actorSeguro)
                });

                if (Convert.ToInt32(resultado) == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "El enlace de recuperación no es válido o ha expirado.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Message = "Contraseña actualizada correctamente.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al restablecer contraseña");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al restablecer la contraseña.";
            }

            return mr;
        }
    }
}
