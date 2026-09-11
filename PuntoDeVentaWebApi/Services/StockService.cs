using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Inventario;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    /// <summary>Servicio del backend para inventario (stock por sucursal y movimientos).</summary>
    public class StockService
    {
        private readonly DbWrapper _dbWrapper;

        public StockService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<List<StockDTO>> ObtenerStockPorSucursal(long empresaId, long sucursalId)
        {
            try
            {
                Log.Information("StockService.ObtenerStockPorSucursal {SucursalId}", sucursalId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (sucursalId <= 0)
                {
                    throw new ArgumentException("El identificador de Sucursal es inválido.");
                }

                return _dbWrapper.ObtenerStockPorSucursal(empresaId, sucursalId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerStockPorSucursal");
                return new ModelResponse<List<StockDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerStockPorSucursal");
                return new ModelResponse<List<StockDTO>> { IsSuccess = false, Message = "Ocurrió un error al obtener el stock." };
            }
        }

        public ModelResponse<StockDTO> ObtenerStock(long empresaId, long sucursalId, long productoId)
        {
            try
            {
                Log.Information("StockService.ObtenerStock producto {ProductoId}", productoId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (sucursalId <= 0 || productoId <= 0)
                {
                    throw new ArgumentException("La sucursal y el producto son requeridos.");
                }

                return _dbWrapper.ObtenerStock(empresaId, sucursalId, productoId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerStock");
                return new ModelResponse<StockDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerStock");
                return new ModelResponse<StockDTO> { IsSuccess = false, Message = "Ocurrió un error al consultar el stock." };
            }
        }

        public ModelResponse<List<StockMovimientoDTO>> ObtenerMovimientos(long empresaId, long sucursalId, long productoId)
        {
            try
            {
                Log.Information("StockService.ObtenerMovimientos");
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                return _dbWrapper.ObtenerMovimientos(empresaId, sucursalId, productoId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerMovimientos");
                return new ModelResponse<List<StockMovimientoDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerMovimientos");
                return new ModelResponse<List<StockMovimientoDTO>> { IsSuccess = false, Message = "Ocurrió un error al obtener los movimientos." };
            }
        }

        public ModelResponse<decimal> RegistrarMovimientoStock(long empresaId, long sucursalId, long productoId,
            string tipoMovimiento, decimal cantidad, string motivo, long? referenciaId, string usuario)
        {
            try
            {
                Log.Information("StockService.RegistrarMovimientoStock {TipoMovimiento}", tipoMovimiento);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (sucursalId <= 0 || productoId <= 0)
                {
                    throw new ArgumentException("La sucursal y el producto son requeridos.");
                }

                if (string.IsNullOrWhiteSpace(tipoMovimiento))
                {
                    throw new ArgumentException("El tipo de movimiento es requerido.");
                }

                if (cantidad == 0)
                {
                    throw new ArgumentException("La cantidad del movimiento no puede ser cero.");
                }

                return _dbWrapper.RegistrarMovimientoStock(empresaId, sucursalId, productoId, tipoMovimiento,
                    cantidad, motivo, referenciaId, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en RegistrarMovimientoStock");
                return new ModelResponse<decimal> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en RegistrarMovimientoStock");
                return new ModelResponse<decimal> { IsSuccess = false, Message = "Ocurrió un error al registrar el movimiento de stock." };
            }
        }
    }
}
