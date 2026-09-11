using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Caja;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    /// <summary>Servicio del backend para caja chica por usuario+sucursal y salidas de caja.</summary>
    public class CajaChicaService
    {
        private readonly DbWrapper _dbWrapper;

        public CajaChicaService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<CajaChicaDTO> ObtenerCajaChicaAbierta(long empresaId, long sucursalId, long usuarioId)
        {
            try
            {
                Log.Information("CajaChicaService.ObtenerCajaChicaAbierta sucursal {SucursalId}", sucursalId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (sucursalId <= 0)
                {
                    throw new ArgumentException("El identificador de Sucursal es inválido.");
                }

                if (usuarioId <= 0)
                {
                    throw new ArgumentException("El identificador de Usuario es inválido.");
                }

                return _dbWrapper.ObtenerCajaChicaAbierta(empresaId, sucursalId, usuarioId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerCajaChicaAbierta");
                return new ModelResponse<CajaChicaDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerCajaChicaAbierta");
                return new ModelResponse<CajaChicaDTO> { IsSuccess = false, Message = "Ocurrió un error al obtener la caja chica abierta." };
            }
        }

        public ModelResponse<CajaChicaDTO> AbrirCajaChica(long empresaId, long sucursalId, long usuarioId, decimal montoInicial, string usuario)
        {
            try
            {
                Log.Information("CajaChicaService.AbrirCajaChica para {Usuario}", usuario);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (sucursalId <= 0)
                {
                    throw new ArgumentException("El identificador de Sucursal es inválido.");
                }

                if (usuarioId <= 0)
                {
                    throw new ArgumentException("El identificador de Usuario es inválido.");
                }

                if (montoInicial < 0)
                {
                    throw new ArgumentException("El monto inicial no puede ser negativo.");
                }

                return _dbWrapper.AbrirCajaChica(empresaId, sucursalId, usuarioId, montoInicial, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en AbrirCajaChica");
                return new ModelResponse<CajaChicaDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en AbrirCajaChica");
                return new ModelResponse<CajaChicaDTO> { IsSuccess = false, Message = "Ocurrió un error al abrir la caja chica." };
            }
        }

        public ModelResponse CerrarCajaChica(long empresaId, long cajaChicaId, string usuario)
        {
            try
            {
                Log.Information("CajaChicaService.CerrarCajaChica {CajaChicaId}", cajaChicaId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (cajaChicaId <= 0)
                {
                    throw new ArgumentException("El identificador de Caja chica es inválido.");
                }

                return _dbWrapper.CerrarCajaChica(empresaId, cajaChicaId, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en CerrarCajaChica");
                return new ModelResponse { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en CerrarCajaChica");
                return new ModelResponse { IsSuccess = false, Message = "Ocurrió un error al cerrar la caja chica." };
            }
        }

        public ModelResponse<SalidaCaja> RegistrarSalidaCaja(long empresaId, SalidaCaja salida, string usuario)
        {
            try
            {
                Log.Information("CajaChicaService.RegistrarSalidaCaja para {Usuario}", usuario);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (salida == null || salida.CajaChicaId <= 0)
                {
                    throw new ArgumentException("La caja chica es requerida.");
                }

                if (salida.Monto <= 0)
                {
                    throw new ArgumentException("El monto de la salida debe ser mayor que cero.");
                }

                return _dbWrapper.RegistrarSalidaCaja(empresaId, salida, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en RegistrarSalidaCaja");
                return new ModelResponse<SalidaCaja> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en RegistrarSalidaCaja");
                return new ModelResponse<SalidaCaja> { IsSuccess = false, Message = "Ocurrió un error al registrar la salida de caja." };
            }
        }

        public ModelResponse<List<SalidaCaja>> ObtenerSalidasCaja(long empresaId, long cajaChicaId)
        {
            try
            {
                Log.Information("CajaChicaService.ObtenerSalidasCaja {CajaChicaId}", cajaChicaId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (cajaChicaId <= 0)
                {
                    throw new ArgumentException("El identificador de Caja chica es inválido.");
                }

                return _dbWrapper.ObtenerSalidasCaja(empresaId, cajaChicaId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerSalidasCaja");
                return new ModelResponse<List<SalidaCaja>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerSalidasCaja");
                return new ModelResponse<List<SalidaCaja>> { IsSuccess = false, Message = "Ocurrió un error al obtener las salidas de caja." };
            }
        }
    }
}
