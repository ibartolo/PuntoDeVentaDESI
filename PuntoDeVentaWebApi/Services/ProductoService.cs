using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    /// <summary>Servicio del backend para la administración de productos.</summary>
    public class ProductoService
    {
        private readonly DbWrapper _dbWrapper;

        public ProductoService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<List<ProductoDTO>> ObtenerProductos(long empresaId)
        {
            try
            {
                Log.Information("ProductoService.ObtenerProductos para empresa {EmpresaId}", empresaId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                return _dbWrapper.ObtenerProductos(empresaId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerProductos");
                return new ModelResponse<List<ProductoDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerProductos");
                return new ModelResponse<List<ProductoDTO>> { IsSuccess = false, Message = "Ocurrió un error al obtener los productos." };
            }
        }

        public ModelResponse<ProductoDTO> ObtenerProductoPorId(long empresaId, long productoId)
        {
            try
            {
                Log.Information("ProductoService.ObtenerProductoPorId {ProductoId}", productoId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (productoId <= 0)
                {
                    throw new ArgumentException("El identificador de Producto es inválido.");
                }

                return _dbWrapper.ObtenerProductoPorId(empresaId, productoId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerProductoPorId");
                return new ModelResponse<ProductoDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerProductoPorId");
                return new ModelResponse<ProductoDTO> { IsSuccess = false, Message = "Ocurrió un error al consultar el producto." };
            }
        }

        public ModelResponse<ProductoDTO> ObtenerProductoPorCodigo(long empresaId, string codigo)
        {
            try
            {
                Log.Information("ProductoService.ObtenerProductoPorCodigo {Codigo}", codigo);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (string.IsNullOrWhiteSpace(codigo))
                {
                    throw new ArgumentException("El código es requerido.");
                }

                return _dbWrapper.ObtenerProductoPorCodigo(empresaId, codigo.Trim());
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerProductoPorCodigo");
                return new ModelResponse<ProductoDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerProductoPorCodigo");
                return new ModelResponse<ProductoDTO> { IsSuccess = false, Message = "Ocurrió un error al consultar el producto por código." };
            }
        }

        public ModelResponse<ProductoDTO> GuardarOActualizarProducto(long empresaId, Producto producto, string usuario)
        {
            try
            {
                Log.Information("ProductoService.GuardarOActualizarProducto para {Usuario}", usuario);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (producto == null || string.IsNullOrWhiteSpace(producto.Nombre))
                {
                    throw new ArgumentException("El Nombre es requerido.");
                }

                return _dbWrapper.GuardarOActualizarProducto(empresaId, producto, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarOActualizarProducto");
                return new ModelResponse<ProductoDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarOActualizarProducto");
                return new ModelResponse<ProductoDTO> { IsSuccess = false, Message = "Ocurrió un error al guardar el producto." };
            }
        }

        public ModelResponse EliminarProducto(long empresaId, long productoId, string usuario)
        {
            try
            {
                Log.Information("ProductoService.EliminarProducto {ProductoId}", productoId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (productoId <= 0)
                {
                    throw new ArgumentException("El identificador de Producto es inválido.");
                }

                return _dbWrapper.EliminarProducto(empresaId, productoId, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en EliminarProducto");
                return new ModelResponse { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en EliminarProducto");
                return new ModelResponse { IsSuccess = false, Message = "Ocurrió un error al desactivar el producto." };
            }
        }
    }
}
