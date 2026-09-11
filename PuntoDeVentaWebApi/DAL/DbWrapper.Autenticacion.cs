using System;
using System.Data;
using System.Data.SqlClient;
using PuntoDeVentaEntities.Autenticacion;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Helpers;
using Serilog;

namespace PuntoDeVentaWebApi.DAL
{
    public partial class DbWrapper
    {
        public ModelResponse<Usuario> AutenticarUsuario(string nombreUsuario, string password)
        {
            var mr = new ModelResponse<Usuario>();
            try
            {
                if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrEmpty(password))
                {
                    mr.IsSuccess = false;
                    mr.Message = "Usuario o contraseña incorrectos.";
                    return mr;
                }

                var usuario = GetObject(
                    "sp_Usuario_ObtenerParaLogin",
                    new Func<IDataReader, Usuario>(r => LlenarEntidad<Usuario>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@NombreUsuario", nombreUsuario.Trim()) });

                if (usuario == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Usuario o contraseña incorrectos.";
                    return mr;
                }

                var passwordOk = Cryptography.VerifyPassword(
                    password,
                    usuario.ContrasenaHash,
                    usuario.ContrasenaSalt,
                    usuario.ContrasenaIteraciones);

                if (!passwordOk)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Usuario o contraseña incorrectos.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = usuario;
                mr.Message = "Autenticación correcta";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al autenticar usuario {NombreUsuario}", nombreUsuario);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al autenticar el usuario.";
            }

            return mr;
        }
    }
}
