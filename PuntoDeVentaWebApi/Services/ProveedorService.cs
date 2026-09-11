using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    public class ProveedorService
    {
        private readonly DbWrapper _dbWrapper;

        public ProveedorService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<List<Proveedor>> ObtenerProveedores(long empresaId)
        {
            try
            {
                Log.Information("ProveedorService.ObtenerProveedores para empresa {EmpresaId}", empresaId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                return _dbWrapper.ObtenerProveedores(empresaId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerProveedores");
                return new ModelResponse<List<Proveedor>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerProveedores");
                return new ModelResponse<List<Proveedor>> { IsSuccess = false, Message = "Ocurrió un error al obtener los proveedores." };
            }
        }

        public ModelResponse<Proveedor> ObtenerProveedorPorId(long empresaId, long proveedorId)
        {
            try
            {
                Log.Information("ProveedorService.ObtenerProveedorPorId {ProveedorId}", proveedorId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (proveedorId <= 0)
                {
                    throw new ArgumentException("El identificador de Proveedor es inválido.");
                }

                return _dbWrapper.ObtenerProveedorPorId(empresaId, proveedorId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerProveedorPorId");
                return new ModelResponse<Proveedor> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerProveedorPorId");
                return new ModelResponse<Proveedor> { IsSuccess = false, Message = "Ocurrió un error al consultar el proveedor." };
            }
        }

        public ModelResponse<Proveedor> GuardarOActualizarProveedor(long empresaId, Proveedor proveedor, string usuario)
        {
            try
            {
                Log.Information("ProveedorService.GuardarOActualizarProveedor para {Usuario}", usuario);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (proveedor == null || string.IsNullOrWhiteSpace(proveedor.Nombre))
                {
                    throw new ArgumentException("El Nombre es requerido.");
                }

                return _dbWrapper.GuardarOActualizarProveedor(empresaId, proveedor, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarOActualizarProveedor");
                return new ModelResponse<Proveedor> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarOActualizarProveedor");
                return new ModelResponse<Proveedor> { IsSuccess = false, Message = "Ocurrió un error al guardar el proveedor." };
            }
        }

        public ModelResponse EliminarProveedor(long empresaId, long proveedorId, string usuario)
        {
            try
            {
                Log.Information("ProveedorService.EliminarProveedor {ProveedorId}", proveedorId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (proveedorId <= 0)
                {
                    throw new ArgumentException("El identificador de Proveedor es inválido.");
                }

                return _dbWrapper.EliminarProveedor(empresaId, proveedorId, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en EliminarProveedor");
                return new ModelResponse { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en EliminarProveedor");
                return new ModelResponse { IsSuccess = false, Message = "Ocurrió un error al desactivar el proveedor." };
            }
        }
    }
}
