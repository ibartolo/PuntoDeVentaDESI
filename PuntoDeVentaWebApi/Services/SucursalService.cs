using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    public class SucursalService
    {
        private readonly DbWrapper _dbWrapper;

        public SucursalService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<List<Sucursal>> ObtenerSucursales(long empresaId)
        {
            try
            {
                Log.Information("SucursalService.ObtenerSucursales para empresa {EmpresaId}", empresaId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                return _dbWrapper.ObtenerSucursales(empresaId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerSucursales");
                return new ModelResponse<List<Sucursal>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerSucursales");
                return new ModelResponse<List<Sucursal>> { IsSuccess = false, Message = "Ocurrió un error al obtener las sucursales." };
            }
        }

        public ModelResponse<Sucursal> ObtenerSucursalPorId(long empresaId, long sucursalId)
        {
            try
            {
                Log.Information("SucursalService.ObtenerSucursalPorId {SucursalId}", sucursalId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (sucursalId <= 0)
                {
                    throw new ArgumentException("El identificador de Sucursal es inválido.");
                }

                return _dbWrapper.ObtenerSucursalPorId(empresaId, sucursalId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerSucursalPorId");
                return new ModelResponse<Sucursal> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerSucursalPorId");
                return new ModelResponse<Sucursal> { IsSuccess = false, Message = "Ocurrió un error al consultar la sucursal." };
            }
        }

        public ModelResponse<Sucursal> GuardarOActualizarSucursal(long empresaId, Sucursal sucursal, string usuario)
        {
            try
            {
                Log.Information("SucursalService.GuardarOActualizarSucursal para {Usuario}", usuario);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (sucursal == null || string.IsNullOrWhiteSpace(sucursal.Nombre))
                {
                    throw new ArgumentException("El Nombre es requerido.");
                }

                return _dbWrapper.GuardarOActualizarSucursal(empresaId, sucursal, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarOActualizarSucursal");
                return new ModelResponse<Sucursal> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarOActualizarSucursal");
                return new ModelResponse<Sucursal> { IsSuccess = false, Message = "Ocurrió un error al guardar la sucursal." };
            }
        }

        public ModelResponse EliminarSucursal(long empresaId, long sucursalId, string usuario)
        {
            try
            {
                Log.Information("SucursalService.EliminarSucursal {SucursalId}", sucursalId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (sucursalId <= 0)
                {
                    throw new ArgumentException("El identificador de Sucursal es inválido.");
                }

                return _dbWrapper.EliminarSucursal(empresaId, sucursalId, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en EliminarSucursal");
                return new ModelResponse { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en EliminarSucursal");
                return new ModelResponse { IsSuccess = false, Message = "Ocurrió un error al desactivar la sucursal." };
            }
        }
    }
}
