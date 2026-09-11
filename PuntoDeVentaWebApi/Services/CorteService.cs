using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Caja;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    /// <summary>Servicio del backend para cortes de caja.</summary>
    public class CorteService
    {
        private readonly DbWrapper _dbWrapper;

        public CorteService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<List<CorteDTO>> ObtenerCortes(long empresaId, long sucursalId)
        {
            try
            {
                Log.Information("CorteService.ObtenerCortes");
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                return _dbWrapper.ObtenerCortes(empresaId, sucursalId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerCortes");
                return new ModelResponse<List<CorteDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerCortes");
                return new ModelResponse<List<CorteDTO>> { IsSuccess = false, Message = "Ocurrió un error al obtener los cortes." };
            }
        }

        public ModelResponse<CorteDTO> ObtenerCortePorId(long empresaId, long corteId)
        {
            try
            {
                Log.Information("CorteService.ObtenerCortePorId {CorteId}", corteId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (corteId <= 0)
                {
                    throw new ArgumentException("El identificador de Corte es inválido.");
                }

                return _dbWrapper.ObtenerCortePorId(empresaId, corteId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerCortePorId");
                return new ModelResponse<CorteDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerCortePorId");
                return new ModelResponse<CorteDTO> { IsSuccess = false, Message = "Ocurrió un error al consultar el corte." };
            }
        }

        public ModelResponse<CorteDTO> GuardarCorte(long empresaId, CorteDTO corte, string usuario)
        {
            try
            {
                Log.Information("CorteService.GuardarCorte para {Usuario}", usuario);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (corte == null || corte.CajaChicaId <= 0)
                {
                    throw new ArgumentException("La caja chica es requerida para el corte.");
                }

                if (corte.UsuarioId <= 0)
                {
                    throw new ArgumentException("El usuario es requerido para el corte.");
                }

                return _dbWrapper.GuardarCorte(empresaId, corte, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarCorte");
                return new ModelResponse<CorteDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarCorte");
                return new ModelResponse<CorteDTO> { IsSuccess = false, Message = "Ocurrió un error al guardar el corte." };
            }
        }
    }
}
