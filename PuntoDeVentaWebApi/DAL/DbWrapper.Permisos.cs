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
        // =========================================
        // Permisos (RolPaginaAccion)
        // =========================================

        /// <summary>Obtiene los permisos efectivos de un usuario por su nombre (sp_Permisos_PorUsuario).</summary>
        public ModelResponse<List<PermisosViewModel>> ObtenerPermisosPorUsuario(string nombreUsuario)
        {
            var mr = new ModelResponse<List<PermisosViewModel>>();
            try
            {
                if (string.IsNullOrWhiteSpace(nombreUsuario))
                {
                    mr.IsSuccess = false;
                    mr.Message = "El nombre de usuario es requerido.";
                    return mr;
                }

                var permisos = GetObjects(
                    "sp_Permisos_PorUsuario",
                    new Func<IDataReader, PermisosViewModel>(r => LlenarEntidad<PermisosViewModel>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@NombreUsuario", nombreUsuario.Trim()) });

                mr.IsSuccess = true;
                mr.Response = permisos.ToList();
                mr.Message = "Permisos obtenidos correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener permisos para usuario {NombreUsuario}", nombreUsuario);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener los permisos.";
            }

            return mr;
        }

        /// <summary>Valida si un usuario tiene permiso sobre una página/acción por nombre (sp_RolPaginaAccion_Validar).</summary>
        public ModelResponse<bool> ValidarPermisoUsuario(long empresaId, long usuarioId, string paginaNombre, string accion)
        {
            var mr = new ModelResponse<bool>();
            try
            {
                if (empresaId <= 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No se pudo resolver la empresa autenticada.";
                    return mr;
                }

                if (usuarioId <= 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No se pudo resolver el usuario autenticado.";
                    return mr;
                }

                if (string.IsNullOrWhiteSpace(paginaNombre))
                {
                    mr.IsSuccess = false;
                    mr.Message = "El nombre de la página es requerido.";
                    return mr;
                }

                if (string.IsNullOrWhiteSpace(accion))
                {
                    mr.IsSuccess = false;
                    mr.Message = "La acción es requerida.";
                    return mr;
                }

                var pars = new[]
                {
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@UsuarioId", usuarioId),
                    new SqlParameter("@PaginaNombre", paginaNombre.Trim()),
                    new SqlParameter("@Accion", accion.Trim())
                };

                var resultado = ExecuteScalar("sp_RolPaginaAccion_Validar", CommandType.StoredProcedure, pars);
                var tienePermiso = resultado != null && resultado != DBNull.Value && Convert.ToInt32(resultado) == 1;

                mr.IsSuccess = true;
                mr.Response = tienePermiso;
                mr.Message = tienePermiso ? "Permiso concedido." : "Permiso denegado.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al validar permiso para usuario {UsuarioId}, página {PaginaNombre}, acción {Accion}",
                    usuarioId, paginaNombre, accion);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al validar el permiso.";
            }

            return mr;
        }

        /// <summary>Valida si un usuario tiene permiso sobre una página/acción por Id (sp_Permisos_Validar).</summary>
        public ModelResponse<bool> ValidarPermisoPorPagina(long usuarioId, long paginaId, string accion)
        {
            var mr = new ModelResponse<bool>();
            try
            {
                var pars = new[]
                {
                    new SqlParameter("@UsuarioId", usuarioId),
                    new SqlParameter("@PaginaId", paginaId),
                    new SqlParameter("@Accion", accion)
                };

                var resultado = ExecuteScalar("sp_Permisos_Validar", CommandType.StoredProcedure, pars);
                var tienePermiso = resultado != null && resultado != DBNull.Value && Convert.ToInt32(resultado) == 1;

                mr.IsSuccess = true;
                mr.Response = tienePermiso;
                mr.Message = tienePermiso ? "Permiso concedido." : "Permiso denegado.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al validar permiso para usuario {UsuarioId}, página {PaginaId}, acción {Accion}",
                    usuarioId, paginaId, accion);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al validar el permiso.";
            }

            return mr;
        }

        /// <summary>Lista los permisos de un rol (sp_RolPaginaAccion_ListarPorRol).</summary>
        public ModelResponse<List<RolPaginaAccionDTO>> ObtenerPermisosPorRol(long empresaId, long rolId)
        {
            var mr = new ModelResponse<List<RolPaginaAccionDTO>>();
            try
            {
                var permisos = GetObjects(
                    "sp_RolPaginaAccion_ListarPorRol",
                    new Func<IDataReader, RolPaginaAccionDTO>(r => LlenarEntidad<RolPaginaAccionDTO>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@RolId", rolId)
                    });

                mr.IsSuccess = true;
                mr.Response = permisos.ToList();
                mr.Message = "Permisos del rol obtenidos correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener permisos del rol {RolId}", rolId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener los permisos del rol.";
            }

            return mr;
        }

        /// <summary>Guarda (inserta o actualiza) el permiso de un rol sobre una página (sp_RolPaginaAccion_Guardar).</summary>
        public ModelResponse GuardarPermisosRol(long empresaId, long rolId, long paginaId, bool puedeLeer,
            bool puedeCrear, bool puedeEditar, bool puedeEliminar, bool puedeExportar, string usuario)
        {
            var mr = new ModelResponse();
            try
            {
                var actor = NormalizarActor(usuario);
                var pars = new[]
                {
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@RolId", rolId),
                    new SqlParameter("@PaginaId", paginaId),
                    new SqlParameter("@PuedeLeer", puedeLeer),
                    new SqlParameter("@PuedeCrear", puedeCrear),
                    new SqlParameter("@PuedeEditar", puedeEditar),
                    new SqlParameter("@PuedeEliminar", puedeEliminar),
                    new SqlParameter("@PuedeExportar", puedeExportar),
                    new SqlParameter("@Actor", actor)
                };

                var resultado = ExecuteScalar("sp_RolPaginaAccion_Guardar", CommandType.StoredProcedure, pars);
                var id = resultado == null || resultado == DBNull.Value ? 0L : Convert.ToInt64(resultado);

                if (id <= 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No se pudieron guardar los permisos.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = id;
                mr.Message = "Permisos guardados correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar permisos para rol {RolId}, página {PaginaId}", rolId, paginaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar los permisos.";
            }

            return mr;
        }

        /// <summary>Desactiva lógicamente un permiso rol-página (sp_RolPaginaAccion_EliminarLogico).</summary>
        public ModelResponse EliminarPermisoRol(long empresaId, long id, string usuario)
        {
            var mr = new ModelResponse();
            try
            {
                var actor = NormalizarActor(usuario);
                var pars = new[]
                {
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Actor", actor)
                };

                var affected = ExecuteScalar("sp_RolPaginaAccion_EliminarLogico", CommandType.StoredProcedure, pars);
                if (affected != null && Convert.ToInt32(affected) == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Permiso no encontrado o ya inactivo.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Message = "Permiso desactivado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar permiso {Id}", id);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al desactivar el permiso.";
            }

            return mr;
        }

        /// <summary>Conteo de páginas asignadas por rol (sp_RolPaginaAccion_ConteoPorRol).</summary>
        public ModelResponse<List<RolConteoPaginasDTO>> ObtenerConteoPaginasPorRol()
        {
            var mr = new ModelResponse<List<RolConteoPaginasDTO>>();
            try
            {
                var conteo = GetObjects(
                    "sp_RolPaginaAccion_ConteoPorRol",
                    new Func<IDataReader, RolConteoPaginasDTO>(r => LlenarEntidad<RolConteoPaginasDTO>(r)),
                    CommandType.StoredProcedure,
                    null);

                mr.IsSuccess = true;
                mr.Response = conteo.ToList();
                mr.Message = "Conteo de páginas por rol obtenido correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener el conteo de páginas por rol");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener el conteo de páginas.";
            }

            return mr;
        }

        /// <summary>
        /// Reemplaza todos los permisos de un rol en una única transacción
        /// (sp_RolPaginaAccion_EliminarPorRol + sp_RolPaginaAccion_Guardar).
        /// </summary>
        public ModelResponse GuardarPermisosRolMasivo(long empresaId, long rolId, List<PermisoRequest> permisos, string usuario)
        {
            var mr = new ModelResponse();
            try
            {
                var actor = NormalizarActor(usuario);

                BeginTransaction();

                ExecuteScalar(
                    "sp_RolPaginaAccion_EliminarPorRol",
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@RolId", rolId),
                        new SqlParameter("@Actor", actor)
                    });

                if (permisos != null)
                {
                    foreach (var permiso in permisos)
                    {
                        var pars = new[]
                        {
                            new SqlParameter("@EmpresaId", empresaId),
                            new SqlParameter("@RolId", rolId),
                            new SqlParameter("@PaginaId", permiso.PaginaId),
                            new SqlParameter("@PuedeLeer", permiso.PuedeLeer),
                            new SqlParameter("@PuedeCrear", permiso.PuedeCrear),
                            new SqlParameter("@PuedeEditar", permiso.PuedeEditar),
                            new SqlParameter("@PuedeEliminar", permiso.PuedeEliminar),
                            new SqlParameter("@PuedeExportar", permiso.PuedeExportar),
                            new SqlParameter("@Actor", actor)
                        };

                        var resultado = ExecuteScalar("sp_RolPaginaAccion_Guardar", CommandType.StoredProcedure, pars);
                        if (resultado == null || resultado == DBNull.Value || Convert.ToInt64(resultado) <= 0)
                        {
                            throw new InvalidOperationException("No se pudo guardar el permiso de la página " + permiso.PaginaId + ".");
                        }
                    }
                }

                CommitTransaction();

                mr.IsSuccess = true;
                mr.Message = "Permisos guardados correctamente";
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                Log.Error(ex, "Error al guardar permisos masivos para rol {RolId}", rolId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar los permisos.";
            }

            return mr;
        }

        // =========================================
        // Relaciones Usuario-Página (accesos directos)
        // =========================================

        /// <summary>Guarda o reactiva un acceso directo usuario-página (sp_UsuarioPagina_Guardar).</summary>
        public ModelResponse<UsuarioPagina> GuardarOActualizarRelacion(UsuarioPagina relacion, string usuario)
        {
            var mr = new ModelResponse<UsuarioPagina>();
            try
            {
                if (relacion == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "La relación es requerida.";
                    return mr;
                }

                var actor = NormalizarActor(usuario);
                var pars = new[]
                {
                    new SqlParameter("@Id", relacion.Id),
                    new SqlParameter("@UsuarioId", (object)relacion.UsuarioId ?? DBNull.Value),
                    new SqlParameter("@PaginaId", (object)relacion.PaginaId ?? DBNull.Value),
                    new SqlParameter("@Actor", actor)
                };

                var resultado = ExecuteScalar("sp_UsuarioPagina_Guardar", CommandType.StoredProcedure, pars);
                relacion.Id = resultado == null || resultado == DBNull.Value ? 0L : Convert.ToInt64(resultado);

                if (relacion.Id <= 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No se pudo guardar la relación.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = relacion;
                mr.Message = "Relación guardada correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar la relación usuario-página");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar la relación.";
            }

            return mr;
        }

        /// <summary>Lista los accesos directos a páginas de un usuario (sp_UsuarioPagina_ListarPorUsuario).</summary>
        public ModelResponse<List<UsuarioPagina>> ObtenerUsuarioPaginaPorUsuario(long empresaId, long usuarioId)
        {
            var mr = new ModelResponse<List<UsuarioPagina>>();
            try
            {
                var relaciones = GetObjects(
                    "sp_UsuarioPagina_ListarPorUsuario",
                    new Func<IDataReader, UsuarioPagina>(r => LlenarEntidad<UsuarioPagina>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@UsuarioId", usuarioId)
                    });

                mr.IsSuccess = true;
                mr.Response = relaciones.ToList();
                mr.Message = "Accesos del usuario obtenidos correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener los accesos del usuario {UsuarioId}", usuarioId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener los accesos del usuario.";
            }

            return mr;
        }

        /// <summary>Desactiva lógicamente una relación usuario-página (sp_UsuarioPagina_EliminarLogico).</summary>
        public ModelResponse EliminarUsuarioPagina(long id, string usuario)
        {
            var mr = new ModelResponse();
            try
            {
                var actor = NormalizarActor(usuario);
                var pars = new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Actor", actor)
                };

                var affected = ExecuteScalar("sp_UsuarioPagina_EliminarLogico", CommandType.StoredProcedure, pars);
                if (affected != null && Convert.ToInt32(affected) == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Relación no encontrada o ya inactiva.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Message = "Relación desactivada correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar la relación {Id}", id);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al desactivar la relación.";
            }

            return mr;
        }

        // =========================================
        // Relaciones Usuario-Rol
        // =========================================

        /// <summary>Lista las asignaciones usuario-rol de un usuario (sp_UsuarioRol_ListarPorUsuario).</summary>
        public ModelResponse<List<UsuarioRol>> ObtenerUsuarioRolesPorUsuario(long empresaId, long usuarioId)
        {
            var mr = new ModelResponse<List<UsuarioRol>>();
            try
            {
                var asignaciones = GetObjects(
                    "sp_UsuarioRol_ListarPorUsuario",
                    new Func<IDataReader, UsuarioRol>(r => LlenarEntidad<UsuarioRol>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@UsuarioId", usuarioId)
                    });

                mr.IsSuccess = true;
                mr.Response = asignaciones.ToList();
                mr.Message = "Asignaciones usuario-rol obtenidas correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener asignaciones usuario-rol del usuario {UsuarioId}", usuarioId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener las asignaciones usuario-rol.";
            }

            return mr;
        }

        /// <summary>Asigna un rol a un usuario (sp_UsuarioRol_Guardar).</summary>
        public ModelResponse AsignarRolUsuario(long empresaId, long usuarioId, long rolId, string usuario)
        {
            var mr = new ModelResponse();
            try
            {
                var actor = NormalizarActor(usuario);
                var pars = new[]
                {
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@UsuarioId", usuarioId),
                    new SqlParameter("@RolId", rolId),
                    new SqlParameter("@Actor", actor)
                };

                var resultado = ExecuteScalar("sp_UsuarioRol_Guardar", CommandType.StoredProcedure, pars);
                var id = resultado == null || resultado == DBNull.Value ? 0L : Convert.ToInt64(resultado);

                if (id <= 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No se pudo asignar el rol al usuario.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = id;
                mr.Message = "Rol asignado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al asignar rol {RolId} al usuario {UsuarioId}", rolId, usuarioId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al asignar el rol.";
            }

            return mr;
        }

        /// <summary>Elimina lógicamente la asignación de un rol a un usuario (sp_UsuarioRol_EliminarLogico).</summary>
        public ModelResponse EliminarUsuarioRol(long empresaId, long usuarioRolId, string usuario)
        {
            var mr = new ModelResponse();
            try
            {
                var actor = NormalizarActor(usuario);
                var pars = new[]
                {
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@UsuarioRolId", usuarioRolId),
                    new SqlParameter("@Actor", actor)
                };

                var affected = ExecuteScalar("sp_UsuarioRol_EliminarLogico", CommandType.StoredProcedure, pars);
                if (affected != null && Convert.ToInt32(affected) == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Asignación no encontrada o ya inactiva.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Message = "Rol eliminado del usuario correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar la asignación usuario-rol {UsuarioRolId}", usuarioRolId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al eliminar el rol del usuario.";
            }

            return mr;
        }
    }
}
