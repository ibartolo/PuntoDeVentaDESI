using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    /// <summary>Servicio del backend para la administración de roles.</summary>
    public class RolService
    {
        private readonly DbWrapper _dbWrapper;

        public RolService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<List<Rol>> ObtenerRoles(string usuario)
        {
            try
            {
                Log.Information("RolService.ObtenerRoles para usuario {Usuario}", usuario);
                if (string.IsNullOrWhiteSpace(usuario))
                {
                    throw new ArgumentException("El nombre de usuario es requerido.");
                }

                return _dbWrapper.ObtenerRoles(usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerRoles");
                return new ModelResponse<List<Rol>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerRoles");
                return new ModelResponse<List<Rol>> { IsSuccess = false, Message = "Ocurrió un error al obtener los roles." };
            }
        }

        public ModelResponse<Rol> ObtenerRolPorId(long empresaId, long rolId)
        {
            try
            {
                Log.Information("RolService.ObtenerRolPorId {RolId}", rolId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (rolId <= 0)
                {
                    throw new ArgumentException("El identificador de Rol es inválido.");
                }

                return _dbWrapper.ObtenerRolPorId(empresaId, rolId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerRolPorId");
                return new ModelResponse<Rol> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerRolPorId");
                return new ModelResponse<Rol> { IsSuccess = false, Message = "Ocurrió un error al obtener el rol." };
            }
        }

        public ModelResponse<List<Rol>> ObtenerRolesPorUsuario(long empresaId, long usuarioId)
        {
            try
            {
                Log.Information("RolService.ObtenerRolesPorUsuario {UsuarioId}", usuarioId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (usuarioId <= 0)
                {
                    throw new ArgumentException("El identificador de Usuario es inválido.");
                }

                return _dbWrapper.ObtenerRolesPorUsuario(empresaId, usuarioId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerRolesPorUsuario");
                return new ModelResponse<List<Rol>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerRolesPorUsuario");
                return new ModelResponse<List<Rol>> { IsSuccess = false, Message = "Ocurrió un error al obtener los roles del usuario." };
            }
        }

        public ModelResponse<Rol> GuardarOActualizarRol(long empresaId, Rol rol, string usuario)
        {
            try
            {
                Log.Information("RolService.GuardarOActualizarRol para {Usuario}", usuario);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (rol == null || string.IsNullOrWhiteSpace(rol.Nombre))
                {
                    throw new ArgumentException("El Nombre del rol es requerido.");
                }

                return _dbWrapper.GuardarOActualizarRol(empresaId, rol, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarOActualizarRol");
                return new ModelResponse<Rol> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarOActualizarRol");
                return new ModelResponse<Rol> { IsSuccess = false, Message = "Ocurrió un error al guardar el rol." };
            }
        }

        public ModelResponse EliminarRol(long empresaId, long rolId, string usuario)
        {
            try
            {
                Log.Information("RolService.EliminarRol {RolId}", rolId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (rolId <= 0)
                {
                    throw new ArgumentException("El identificador de Rol es inválido.");
                }

                return _dbWrapper.EliminarRol(empresaId, rolId, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en EliminarRol");
                return new ModelResponse { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en EliminarRol");
                return new ModelResponse { IsSuccess = false, Message = "Ocurrió un error al desactivar el rol." };
            }
        }
    }
}
