using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    /// <summary>Servicio del backend para el historial de precios por producto.</summary>
    public class PrecioService
    {
        private readonly DbWrapper _dbWrapper;

        public PrecioService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<PrecioDTO> ObtenerPrecioActivo(long empresaId, long productoId)
        {
            try
            {
                Log.Information("PrecioService.ObtenerPrecioActivo {ProductoId}", productoId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (productoId <= 0)
                {
                    throw new ArgumentException("El identificador de Producto es inválido.");
                }

                return _dbWrapper.ObtenerPrecioActivo(empresaId, productoId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerPrecioActivo");
                return new ModelResponse<PrecioDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerPrecioActivo");
                return new ModelResponse<PrecioDTO> { IsSuccess = false, Message = "Ocurrió un error al obtener el precio activo." };
            }
        }

        public ModelResponse<List<PrecioDTO>> ObtenerPreciosPorProducto(long empresaId, long productoId)
        {
            try
            {
                Log.Information("PrecioService.ObtenerPreciosPorProducto {ProductoId}", productoId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (productoId <= 0)
                {
                    throw new ArgumentException("El identificador de Producto es inválido.");
                }

                return _dbWrapper.ObtenerPreciosPorProducto(empresaId, productoId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerPreciosPorProducto");
                return new ModelResponse<List<PrecioDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerPreciosPorProducto");
                return new ModelResponse<List<PrecioDTO>> { IsSuccess = false, Message = "Ocurrió un error al obtener el historial de precios." };
            }
        }

        public ModelResponse<PrecioDTO> GuardarOActualizarPrecio(long empresaId, Precio precio, string usuario)
        {
            try
            {
                Log.Information("PrecioService.GuardarOActualizarPrecio para {Usuario}", usuario);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (precio == null || precio.ProductoId <= 0)
                {
                    throw new ArgumentException("El producto es requerido.");
                }

                if (precio.PrecioVenta < 0)
                {
                    throw new ArgumentException("El precio de venta no puede ser negativo.");
                }

                return _dbWrapper.GuardarOActualizarPrecio(empresaId, precio, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarOActualizarPrecio");
                return new ModelResponse<PrecioDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarOActualizarPrecio");
                return new ModelResponse<PrecioDTO> { IsSuccess = false, Message = "Ocurrió un error al guardar el precio." };
            }
        }
    }
}
