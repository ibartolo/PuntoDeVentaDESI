using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    /// <summary>Servicio del backend para la administración de páginas/menús.</summary>
    public class PaginaService
    {
        private readonly DbWrapper _dbWrapper;

        public PaginaService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<List<Pagina>> ObtenerPaginas()
        {
            try
            {
                Log.Information("PaginaService.ObtenerPaginas");
                return _dbWrapper.ObtenerPaginas();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerPaginas");
                return new ModelResponse<List<Pagina>> { IsSuccess = false, Message = "Ocurrió un error al obtener las páginas." };
            }
        }

        public ModelResponse<Pagina> ObtenerPaginaPorId(long paginaId)
        {
            try
            {
                Log.Information("PaginaService.ObtenerPaginaPorId {PaginaId}", paginaId);
                if (paginaId <= 0)
                {
                    throw new ArgumentException("El identificador de Página es inválido.");
                }

                return _dbWrapper.ObtenerPaginaPorId(paginaId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerPaginaPorId");
                return new ModelResponse<Pagina> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerPaginaPorId");
                return new ModelResponse<Pagina> { IsSuccess = false, Message = "Ocurrió un error al obtener la página." };
            }
        }

        public ModelResponse<List<Pagina>> ObtenerPaginasPorUsuario(long empresaId, long usuarioId)
        {
            try
            {
                Log.Information("PaginaService.ObtenerPaginasPorUsuario {UsuarioId}", usuarioId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (usuarioId <= 0)
                {
                    throw new ArgumentException("El identificador de Usuario es inválido.");
                }

                return _dbWrapper.ObtenerPaginasPorUsuario(empresaId, usuarioId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerPaginasPorUsuario");
                return new ModelResponse<List<Pagina>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerPaginasPorUsuario");
                return new ModelResponse<List<Pagina>> { IsSuccess = false, Message = "Ocurrió un error al obtener las páginas del usuario." };
            }
        }

        public ModelResponse<Pagina> GuardarOActualizarPagina(Pagina pagina, string usuario)
        {
            try
            {
                Log.Information("PaginaService.GuardarOActualizarPagina para {Usuario}", usuario);
                if (pagina == null || string.IsNullOrWhiteSpace(pagina.Nombre))
                {
                    throw new ArgumentException("El Nombre de la página es requerido.");
                }

                return _dbWrapper.GuardarOActualizarPagina(pagina, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarOActualizarPagina");
                return new ModelResponse<Pagina> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarOActualizarPagina");
                return new ModelResponse<Pagina> { IsSuccess = false, Message = "Ocurrió un error al guardar la página." };
            }
        }

        public ModelResponse EliminarPagina(long paginaId, string usuario)
        {
            try
            {
                Log.Information("PaginaService.EliminarPagina {PaginaId}", paginaId);
                if (paginaId <= 0)
                {
                    throw new ArgumentException("El identificador de Página es inválido.");
                }

                return _dbWrapper.EliminarPagina(paginaId, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en EliminarPagina");
                return new ModelResponse { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en EliminarPagina");
                return new ModelResponse { IsSuccess = false, Message = "Ocurrió un error al desactivar la página." };
            }
        }
    }
}
