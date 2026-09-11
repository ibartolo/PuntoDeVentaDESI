using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using PuntoDeVentaEntities.Seguridad;
using Serilog;

namespace PuntoDeVentaWebApi.DAL
{
    public partial class DbWrapper
    {
        /// <summary>Lista los roles de la empresa del usuario autenticado (sp_Rol_Listar).</summary>
        public ModelResponse<List<Rol>> ObtenerRoles(string usuario)
        {
            var mr = new ModelResponse<List<Rol>>();
            try
            {
                if (string.IsNullOrWhiteSpace(usuario))
                {
                    mr.IsSuccess = false;
                    mr.Message = "El nombre de usuario es requerido.";
                    return mr;
                }

                var roles = GetObjects(
                    "sp_Rol_Listar",
                    new Func<IDataReader, Rol>(r => LlenarEntidad<Rol>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@Usuario", usuario.Trim()) });

                mr.IsSuccess = true;
                mr.Response = roles.ToList();
                mr.Message = "Roles obtenidos correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener roles para usuario {Usuario}", usuario);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener los roles.";
            }

            return mr;
        }

        /// <summary>Obtiene un rol por su Id dentro de la empresa (sp_Rol_Obtener).</summary>
        public ModelResponse<Rol> ObtenerRolPorId(long empresaId, long rolId)
        {
            var mr = new ModelResponse<Rol>();
            try
            {
                var rol = GetObject(
                    "sp_Rol_Obtener",
                    new Func<IDataReader, Rol>(r => LlenarEntidad<Rol>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@RolId", rolId)
                    });

                if (rol == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Rol no encontrado.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = rol;
                mr.Message = "Rol obtenido correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener rol {RolId}", rolId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener el rol.";
            }

            return mr;
        }

        /// <summary>Lista los roles asignados a un usuario (sp_Rol_PorUsuario).</summary>
        public ModelResponse<List<Rol>> ObtenerRolesPorUsuario(long empresaId, long usuarioId)
        {
            var mr = new ModelResponse<List<Rol>>();
            try
            {
                var roles = GetObjects(
                    "sp_Rol_PorUsuario",
                    new Func<IDataReader, Rol>(r => LlenarEntidad<Rol>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@UsuarioId", usuarioId)
                    });

                mr.IsSuccess = true;
                mr.Response = roles.ToList();
                mr.Message = "Roles del usuario obtenidos correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener roles del usuario {UsuarioId}", usuarioId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener los roles del usuario.";
            }

            return mr;
        }

        /// <summary>
        /// Guarda o actualiza un rol (sp_Rol_Guardar). El SP devuelve:
        /// 0 = empresa no válida, -1 = nombre duplicado, > 0 = Id del rol.
        /// </summary>
        public ModelResponse<Rol> GuardarOActualizarRol(long empresaId, Rol rol, string usuario)
        {
            var mr = new ModelResponse<Rol>();
            try
            {
                if (rol == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "El rol es requerido.";
                    return mr;
                }

                var actor = NormalizarActor(usuario ?? rol.CreadoPor ?? rol.ModificadoPor);

                var pars = new[]
                {
                    new SqlParameter("@Id", rol.Id),
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@Nombre", rol.Nombre),
                    new SqlParameter("@Descripcion", (object)rol.Descripcion ?? DBNull.Value),
                    new SqlParameter("@PuedeAutorizar", rol.PuedeAutorizar),
                    new SqlParameter("@Actor", actor)
                };

                var resultado = ExecuteScalar("sp_Rol_Guardar", CommandType.StoredProcedure, pars);
                var resultadoLong = Convert.ToInt64(resultado);

                if (resultadoLong == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No se pudo resolver la empresa autenticada.";
                    return mr;
                }

                if (resultadoLong == -1)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Ya existe un rol con ese nombre en la empresa.";
                    return mr;
                }

                rol.Id = resultadoLong;
                return ObtenerRolPorId(empresaId, rol.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar rol para usuario {Usuario}", usuario);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar el rol.";
            }

            return mr;
        }

        /// <summary>Desactiva lógicamente un rol (sp_Rol_EliminarLogico).</summary>
        public ModelResponse EliminarRol(long empresaId, long rolId, string usuario)
        {
            var mr = new ModelResponse();
            try
            {
                var actor = NormalizarActor(usuario);
                var pars = new[]
                {
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@RolId", rolId),
                    new SqlParameter("@Actor", actor)
                };

                var affected = ExecuteScalar("sp_Rol_EliminarLogico", CommandType.StoredProcedure, pars);
                if (affected != null && Convert.ToInt32(affected) == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Rol no encontrado o ya inactivo.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Message = "Rol desactivado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar rol {RolId}", rolId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al desactivar el rol.";
            }

            return mr;
        }

        /// <summary>Normaliza el actor de auditoría al ancho de columna NVARCHAR(25).</summary>
        private static string NormalizarActor(string actor)
        {
            var valor = (actor ?? "system").Trim();
            return valor.Length > 25 ? valor.Substring(0, 25) : valor;
        }
    }
}
