using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaEntities.Ventas;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    /// <summary>Servicio del backend para cancelaciones y devoluciones de venta.</summary>
    public class CancelacionService
    {
        private readonly DbWrapper _dbWrapper;

        public CancelacionService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<List<CancelacionDTO>> ObtenerCancelaciones(long empresaId, long sucursalId)
        {
            try
            {
                Log.Information("CancelacionService.ObtenerCancelaciones para empresa {EmpresaId}", empresaId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                return _dbWrapper.ObtenerCancelaciones(empresaId, sucursalId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerCancelaciones");
                return new ModelResponse<List<CancelacionDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerCancelaciones");
                return new ModelResponse<List<CancelacionDTO>> { IsSuccess = false, Message = "Ocurrió un error al obtener las cancelaciones." };
            }
        }

        public ModelResponse<CancelacionDTO> ObtenerCancelacionPorId(long empresaId, long cancelacionId)
        {
            try
            {
                Log.Information("CancelacionService.ObtenerCancelacionPorId {CancelacionId}", cancelacionId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (cancelacionId <= 0)
                {
                    throw new ArgumentException("El identificador de Cancelación es inválido.");
                }

                return _dbWrapper.ObtenerCancelacionPorId(empresaId, cancelacionId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerCancelacionPorId");
                return new ModelResponse<CancelacionDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerCancelacionPorId");
                return new ModelResponse<CancelacionDTO> { IsSuccess = false, Message = "Ocurrió un error al consultar la cancelación." };
            }
        }

        public ModelResponse<CancelacionDTO> GuardarCancelacion(long empresaId, CancelacionDTO cancelacion, string usuario)
        {
            try
            {
                Log.Information("CancelacionService.GuardarCancelacion para {Usuario}", usuario);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (cancelacion == null || cancelacion.SucursalId <= 0)
                {
                    throw new ArgumentException("La sucursal es requerida.");
                }

                if (cancelacion.VentaId <= 0)
                {
                    throw new ArgumentException("La venta a cancelar es requerida.");
                }

                if (string.IsNullOrWhiteSpace(cancelacion.Tipo) ||
                    (cancelacion.Tipo != "Total" && cancelacion.Tipo != "Parcial"))
                {
                    throw new ArgumentException("El tipo de cancelación debe ser 'Total' o 'Parcial'.");
                }

                if (cancelacion.Tipo == "Parcial" && (cancelacion.Detalle == null || cancelacion.Detalle.Count == 0))
                {
                    throw new ArgumentException("Una cancelación parcial requiere al menos un producto devuelto.");
                }

                return _dbWrapper.GuardarCancelacion(empresaId, cancelacion, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarCancelacion");
                return new ModelResponse<CancelacionDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarCancelacion");
                return new ModelResponse<CancelacionDTO> { IsSuccess = false, Message = "Ocurrió un error al registrar la cancelación." };
            }
        }
    }
}
