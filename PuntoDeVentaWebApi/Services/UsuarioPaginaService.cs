using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    /// <summary>Servicio del backend para los accesos directos usuario-página.</summary>
    public class UsuarioPaginaService
    {
        private readonly DbWrapper _dbWrapper;

        public UsuarioPaginaService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<List<UsuarioPagina>> ObtenerTodasRelaciones()
        {
            try
            {
                Log.Information("UsuarioPaginaService.ObtenerTodasRelaciones");
                return _dbWrapper.ObtenerTodasRelaciones();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerTodasRelaciones");
                return new ModelResponse<List<UsuarioPagina>> { IsSuccess = false, Message = "Ocurrió un error al obtener las relaciones." };
            }
        }

        public ModelResponse<List<UsuarioPagina>> ObtenerUsuarioPaginaPorUsuario(long empresaId, long usuarioId)
        {
            try
            {
                Log.Information("UsuarioPaginaService.ObtenerUsuarioPaginaPorUsuario {UsuarioId}", usuarioId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (usuarioId <= 0)
                {
                    throw new ArgumentException("El identificador de Usuario es inválido.");
                }

                return _dbWrapper.ObtenerUsuarioPaginaPorUsuario(empresaId, usuarioId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerUsuarioPaginaPorUsuario");
                return new ModelResponse<List<UsuarioPagina>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerUsuarioPaginaPorUsuario");
                return new ModelResponse<List<UsuarioPagina>> { IsSuccess = false, Message = "Ocurrió un error al obtener los accesos del usuario." };
            }
        }

        public ModelResponse<UsuarioPagina> ObtenerRelacionPorId(long id)
        {
            try
            {
                Log.Information("UsuarioPaginaService.ObtenerRelacionPorId {Id}", id);
                if (id <= 0)
                {
                    throw new ArgumentException("El identificador de la relación es inválido.");
                }

                return _dbWrapper.ObtenerRelacionPorId(id);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerRelacionPorId");
                return new ModelResponse<UsuarioPagina> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerRelacionPorId");
                return new ModelResponse<UsuarioPagina> { IsSuccess = false, Message = "Ocurrió un error al obtener la relación." };
            }
        }

        public ModelResponse<UsuarioPagina> GuardarOActualizarRelacion(UsuarioPagina relacion, string usuario)
        {
            try
            {
                Log.Information("UsuarioPaginaService.GuardarOActualizarRelacion");
                if (relacion == null || relacion.UsuarioId == null || relacion.PaginaId == null)
                {
                    throw new ArgumentException("El usuario y la página son requeridos.");
                }

                return _dbWrapper.GuardarOActualizarRelacion(relacion, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarOActualizarRelacion");
                return new ModelResponse<UsuarioPagina> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarOActualizarRelacion");
                return new ModelResponse<UsuarioPagina> { IsSuccess = false, Message = "Ocurrió un error al guardar la relación." };
            }
        }

        public ModelResponse EliminarUsuarioPagina(long id, string usuario)
        {
            try
            {
                Log.Information("UsuarioPaginaService.EliminarUsuarioPagina {Id}", id);
                if (id <= 0)
                {
                    throw new ArgumentException("El identificador de la relación es inválido.");
                }

                return _dbWrapper.EliminarUsuarioPagina(id, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en EliminarUsuarioPagina");
                return new ModelResponse { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en EliminarUsuarioPagina");
                return new ModelResponse { IsSuccess = false, Message = "Ocurrió un error al desactivar la relación." };
            }
        }
    }
}
