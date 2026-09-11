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
        public ModelResponse<List<UsuarioPagina>> ObtenerUsuarioPagina(string usuario)
        {
            var mr = new ModelResponse<List<UsuarioPagina>>();
            try
            {
                var resultado = GetObjects(
                    "sp_UsuarioPagina_ListarPorUsuario",
                    new Func<IDataReader, UsuarioPagina>(r => LlenarEntidad<UsuarioPagina>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@Usuario", usuario) });

                mr.IsSuccess = true;
                mr.Response = resultado.ToList();
                mr.Message = "UsuarioPagina obtenidas correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener UsuarioPagina para usuario {Usuario}", usuario);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener las relaciones usuario-página.";
            }

            return mr;
        }

        public ModelResponse<UsuarioPagina> GuardarOActualizarUsuarioPagina(UsuarioPagina u)
        {
            var mr = new ModelResponse<UsuarioPagina>();
            try
            {
                if (u == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "La relación es requerida.";
                    return mr;
                }

                var actor = (u.CreadoPor ?? u.ModificadoPor ?? "system").Trim();
                if (actor.Length > 25)
                {
                    actor = actor.Substring(0, 25);
                }

                var pars = new[]
                {
                    new SqlParameter("@Id", u.Id),
                    new SqlParameter("@UsuarioId", (object)u.UsuarioId ?? DBNull.Value),
                    new SqlParameter("@PaginaId", (object)u.PaginaId ?? DBNull.Value),
                    new SqlParameter("@Actor", actor)
                };

                var resultado = ExecuteScalar("sp_UsuarioPagina_Guardar", CommandType.StoredProcedure, pars);
                u.Id = Convert.ToInt64(resultado);

                mr.IsSuccess = true;
                mr.Response = u;
                mr.Message = "UsuarioPagina guardado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar UsuarioPagina");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar la relación usuario-página.";
            }

            return mr;
        }

        public ModelResponse<UsuarioPagina> ObtenerUsuarioPaginaPorId(long id, string usuario)
        {
            var mr = new ModelResponse<UsuarioPagina>();
            try
            {
                var resultado = GetObject(
                    "sp_UsuarioPagina_Obtener",
                    new Func<IDataReader, UsuarioPagina>(r => LlenarEntidad<UsuarioPagina>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@Id", id) });

                mr.IsSuccess = true;
                mr.Response = resultado;
                mr.Message = "UsuarioPagina obtenido correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener UsuarioPagina {Id}", id);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener la relación usuario-página.";
            }

            return mr;
        }

        public ModelResponse EliminarUsuarioPagina(long id, string modificadoPor, DateTime fechaModificacion)
        {
            var mr = new ModelResponse();
            try
            {
                var actor = (modificadoPor ?? "system").Trim();
                if (actor.Length > 25)
                {
                    actor = actor.Substring(0, 25);
                }

                ExecuteScalar("sp_UsuarioPagina_EliminarLogico", CommandType.StoredProcedure, new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Actor", actor)
                });

                mr.IsSuccess = true;
                mr.Message = "UsuarioPagina eliminado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar UsuarioPagina {Id}", id);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al eliminar la relación usuario-página.";
            }

            return mr;
        }

        /// <summary>Lista todas las relaciones usuario-página activas (sp_UsuarioPagina_ListarTodos).</summary>
        public ModelResponse<List<UsuarioPagina>> ObtenerTodasRelaciones()
        {
            var mr = new ModelResponse<List<UsuarioPagina>>();
            try
            {
                var relaciones = GetObjects(
                    "sp_UsuarioPagina_ListarTodos",
                    new Func<IDataReader, UsuarioPagina>(r => LlenarEntidad<UsuarioPagina>(r)),
                    CommandType.StoredProcedure,
                    null);

                mr.IsSuccess = true;
                mr.Response = relaciones.ToList();
                mr.Message = "Relaciones obtenidas correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener las relaciones usuario-página");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener las relaciones.";
            }

            return mr;
        }

        /// <summary>Obtiene una relación usuario-página por Id (sp_UsuarioPagina_Obtener).</summary>
        public ModelResponse<UsuarioPagina> ObtenerRelacionPorId(long id)
        {
            var mr = new ModelResponse<UsuarioPagina>();
            try
            {
                var relacion = GetObject(
                    "sp_UsuarioPagina_Obtener",
                    new Func<IDataReader, UsuarioPagina>(r => LlenarEntidad<UsuarioPagina>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@Id", id) });

                if (relacion == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Relación no encontrada.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = relacion;
                mr.Message = "Relación obtenida correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener la relación {Id}", id);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener la relación.";
            }

            return mr;
        }

        public ModelResponse<UsuarioPagina> GuardarOActualizarRelacion(UsuarioPagina r)
        {
            return GuardarOActualizarUsuarioPagina(r);
        }

        public ModelResponse EliminarRelacion(long id, string modificadoPor, DateTime fechaModificacion)
        {
            return EliminarUsuarioPagina(id, modificadoPor, fechaModificacion);
        }
    }
}
