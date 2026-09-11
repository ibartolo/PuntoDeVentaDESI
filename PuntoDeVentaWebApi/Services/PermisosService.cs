using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    /// <summary>Servicio del backend para permisos, relaciones y accesos directos.</summary>
    public class PermisosService
    {
        private readonly DbWrapper _dbWrapper;

        public PermisosService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<List<PermisosViewModel>> ObtenerPermisosPorUsuario(string usuario)
        {
            try
            {
                Log.Information("PermisosService.ObtenerPermisosPorUsuario para usuario {Usuario}", usuario);
                if (string.IsNullOrWhiteSpace(usuario))
                {
                    throw new ArgumentException("El nombre de usuario es requerido.");
                }

                return _dbWrapper.ObtenerPermisosPorUsuario(usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerPermisosPorUsuario");
                return new ModelResponse<List<PermisosViewModel>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerPermisosPorUsuario");
                return new ModelResponse<List<PermisosViewModel>> { IsSuccess = false, Message = "Ocurrió un error al obtener los permisos." };
            }
        }

        public ModelResponse<bool> ValidarPermisoUsuario(long empresaId, long usuarioId, string pagina, string accion)
        {
            try
            {
                Log.Information("PermisosService.ValidarPermisoUsuario usuario {UsuarioId}, página {Pagina}, acción {Accion}",
                    usuarioId, pagina, accion);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (usuarioId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver el usuario autenticado.");
                }

                if (string.IsNullOrWhiteSpace(pagina))
                {
                    throw new ArgumentException("El nombre de la página es requerido.");
                }

                if (string.IsNullOrWhiteSpace(accion))
                {
                    throw new ArgumentException("La acción es requerida.");
                }

                return _dbWrapper.ValidarPermisoUsuario(empresaId, usuarioId, pagina, accion);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ValidarPermisoUsuario");
                return new ModelResponse<bool> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ValidarPermisoUsuario");
                return new ModelResponse<bool> { IsSuccess = false, Message = "Ocurrió un error al validar el permiso." };
            }
        }

        public ModelResponse<List<Pagina>> ObtenerPaginas()
        {
            try
            {
                Log.Information("PermisosService.ObtenerPaginas");
                return _dbWrapper.ObtenerPaginas();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerPaginas");
                return new ModelResponse<List<Pagina>> { IsSuccess = false, Message = "Ocurrió un error al obtener las páginas." };
            }
        }

        public ModelResponse<List<RolPaginaAccionDTO>> ObtenerPermisosPorRol(long empresaId, long rolId)
        {
            try
            {
                Log.Information("PermisosService.ObtenerPermisosPorRol para RolId {RolId}", rolId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (rolId <= 0)
                {
                    throw new ArgumentException("El ID del rol es requerido.");
                }

                return _dbWrapper.ObtenerPermisosPorRol(empresaId, rolId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerPermisosPorRol");
                return new ModelResponse<List<RolPaginaAccionDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerPermisosPorRol");
                return new ModelResponse<List<RolPaginaAccionDTO>> { IsSuccess = false, Message = "Ocurrió un error al obtener los permisos del rol." };
            }
        }

        public ModelResponse GuardarPermisosRol(long empresaId, long rolId, long paginaId, bool puedeLeer,
            bool puedeCrear, bool puedeEditar, bool puedeEliminar, bool puedeExportar, string usuario)
        {
            try
            {
                Log.Information("PermisosService.GuardarPermisosRol para RolId {RolId}, PaginaId {PaginaId}", rolId, paginaId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (rolId <= 0)
                {
                    throw new ArgumentException("El ID del rol es requerido.");
                }

                if (paginaId <= 0)
                {
                    throw new ArgumentException("El ID de la página es requerido.");
                }

                if ((puedeCrear || puedeEditar || puedeEliminar || puedeExportar) && !puedeLeer)
                {
                    throw new ArgumentException("No se pueden asignar permisos de escritura sin permiso de lectura.");
                }

                return _dbWrapper.GuardarPermisosRol(empresaId, rolId, paginaId, puedeLeer, puedeCrear,
                    puedeEditar, puedeEliminar, puedeExportar, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarPermisosRol");
                return new ModelResponse { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarPermisosRol");
                return new ModelResponse { IsSuccess = false, Message = "Ocurrió un error al guardar los permisos." };
            }
        }

        public ModelResponse GuardarPermisosRolMasivo(long empresaId, long rolId, List<PermisoRequest> permisos, string usuario)
        {
            try
            {
                Log.Information("PermisosService.GuardarPermisosRolMasivo para RolId {RolId}", rolId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (rolId <= 0)
                {
                    throw new ArgumentException("El ID del rol es requerido.");
                }

                if (permisos == null)
                {
                    throw new ArgumentException("La lista de permisos es requerida.");
                }

                foreach (var permiso in permisos)
                {
                    if (permiso.PaginaId <= 0)
                    {
                        throw new ArgumentException("El ID de la página es requerido.");
                    }

                    if ((permiso.PuedeCrear || permiso.PuedeEditar || permiso.PuedeEliminar || permiso.PuedeExportar) && !permiso.PuedeLeer)
                    {
                        throw new ArgumentException("No se pueden asignar permisos de escritura sin permiso de lectura para la página " + permiso.PaginaId + ".");
                    }
                }

                return _dbWrapper.GuardarPermisosRolMasivo(empresaId, rolId, permisos, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarPermisosRolMasivo");
                return new ModelResponse { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarPermisosRolMasivo");
                return new ModelResponse { IsSuccess = false, Message = "Ocurrió un error al guardar los permisos." };
            }
        }

        public ModelResponse<List<RolConteoPaginasDTO>> ObtenerConteoPaginasPorRol()
        {
            try
            {
                Log.Information("PermisosService.ObtenerConteoPaginasPorRol");
                return _dbWrapper.ObtenerConteoPaginasPorRol();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerConteoPaginasPorRol");
                return new ModelResponse<List<RolConteoPaginasDTO>> { IsSuccess = false, Message = "Ocurrió un error al obtener el conteo de páginas." };
            }
        }

    }
}
