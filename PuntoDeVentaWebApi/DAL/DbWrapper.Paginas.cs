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
        /// <summary>Lista todas las páginas/menús activos (sp_Pagina_Listar).</summary>
        public ModelResponse<List<Pagina>> ObtenerPaginas()
        {
            var mr = new ModelResponse<List<Pagina>>();
            try
            {
                var paginas = GetObjects(
                    "sp_Pagina_Listar",
                    new Func<IDataReader, Pagina>(r => LlenarEntidad<Pagina>(r)),
                    CommandType.StoredProcedure,
                    null);

                mr.IsSuccess = true;
                mr.Response = paginas.ToList();
                mr.Message = "Páginas obtenidas correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener páginas");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener las páginas.";
            }

            return mr;
        }

        /// <summary>Obtiene una página por su Id (sp_Pagina_Obtener).</summary>
        public ModelResponse<Pagina> ObtenerPaginaPorId(long paginaId)
        {
            var mr = new ModelResponse<Pagina>();
            try
            {
                var pagina = GetObject(
                    "sp_Pagina_Obtener",
                    new Func<IDataReader, Pagina>(r => LlenarEntidad<Pagina>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@PaginaId", paginaId) });

                if (pagina == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Página no encontrada.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = pagina;
                mr.Message = "Página obtenida correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener página {PaginaId}", paginaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener la página.";
            }

            return mr;
        }

        /// <summary>Lista las páginas visibles para un usuario según sus roles/accesos directos (sp_Pagina_PorUsuario).</summary>
        public ModelResponse<List<Pagina>> ObtenerPaginasPorUsuario(long empresaId, long usuarioId)
        {
            var mr = new ModelResponse<List<Pagina>>();
            try
            {
                var paginas = GetObjects(
                    "sp_Pagina_PorUsuario",
                    new Func<IDataReader, Pagina>(r => LlenarEntidad<Pagina>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@UsuarioId", usuarioId)
                    });

                mr.IsSuccess = true;
                mr.Response = paginas.ToList();
                mr.Message = "Páginas del usuario obtenidas correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener páginas del usuario {UsuarioId}", usuarioId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener las páginas del usuario.";
            }

            return mr;
        }

        /// <summary>
        /// Guarda o actualiza una página (sp_Pagina_Guardar). El SP devuelve:
        /// -1 = nombre duplicado, 0 = no se pudo actualizar, > 0 = Id de la página.
        /// </summary>
        public ModelResponse<Pagina> GuardarOActualizarPagina(Pagina pagina, string usuario)
        {
            var mr = new ModelResponse<Pagina>();
            try
            {
                if (pagina == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "La página es requerida.";
                    return mr;
                }

                var actor = NormalizarActor(usuario ?? pagina.CreadoPor ?? pagina.ModificadoPor);

                var pars = new[]
                {
                    new SqlParameter("@PaginaId", pagina.Id),
                    new SqlParameter("@Nombre", pagina.Nombre),
                    new SqlParameter("@NombreVisible", (object)pagina.NombreVisible ?? DBNull.Value),
                    new SqlParameter("@Descripcion", (object)pagina.Descripcion ?? DBNull.Value),
                    new SqlParameter("@Tipo", string.IsNullOrWhiteSpace(pagina.Tipo) ? "Menu" : pagina.Tipo),
                    new SqlParameter("@Direccion", (object)pagina.Direccion ?? DBNull.Value),
                    new SqlParameter("@PermisosPadreId", (object)pagina.PermisosPadreId ?? DBNull.Value),
                    new SqlParameter("@Logo", (object)pagina.Logo ?? DBNull.Value),
                    new SqlParameter("@OrdenB", pagina.OrdenB),
                    new SqlParameter("@Actor", actor)
                };

                var resultado = ExecuteScalar("sp_Pagina_Guardar", CommandType.StoredProcedure, pars);
                var resultadoLong = Convert.ToInt64(resultado);

                if (resultadoLong == -1)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Ya existe una página con ese nombre.";
                    return mr;
                }

                if (resultadoLong == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No se pudo guardar la página.";
                    return mr;
                }

                pagina.Id = resultadoLong;
                return ObtenerPaginaPorId(pagina.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar página para usuario {Usuario}", usuario);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar la página.";
            }

            return mr;
        }

        /// <summary>Desactiva lógicamente una página (sp_Pagina_EliminarLogico).</summary>
        public ModelResponse EliminarPagina(long paginaId, string usuario)
        {
            var mr = new ModelResponse();
            try
            {
                var actor = NormalizarActor(usuario);
                var pars = new[]
                {
                    new SqlParameter("@PaginaId", paginaId),
                    new SqlParameter("@Actor", actor)
                };

                var affected = ExecuteScalar("sp_Pagina_EliminarLogico", CommandType.StoredProcedure, pars);
                if (affected != null && Convert.ToInt32(affected) == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Página no encontrada o ya inactiva.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Message = "Página desactivada correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar página {PaginaId}", paginaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al desactivar la página.";
            }

            return mr;
        }
    }
}
