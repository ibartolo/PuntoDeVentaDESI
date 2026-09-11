using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaEntities.Ventas;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    /// <summary>Servicio del backend para ventas POS.</summary>
    public class VentaService
    {
        private readonly DbWrapper _dbWrapper;

        public VentaService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<List<VentaDTO>> ObtenerVentas(long empresaId, long sucursalId)
        {
            try
            {
                Log.Information("VentaService.ObtenerVentas");
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                return _dbWrapper.ObtenerVentas(empresaId, sucursalId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerVentas");
                return new ModelResponse<List<VentaDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerVentas");
                return new ModelResponse<List<VentaDTO>> { IsSuccess = false, Message = "Ocurrió un error al obtener las ventas." };
            }
        }

        public ModelResponse<VentaDTO> ObtenerVentaPorId(long empresaId, long ventaId)
        {
            try
            {
                Log.Information("VentaService.ObtenerVentaPorId {VentaId}", ventaId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (ventaId <= 0)
                {
                    throw new ArgumentException("El identificador de Venta es inválido.");
                }

                return _dbWrapper.ObtenerVentaPorId(empresaId, ventaId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerVentaPorId");
                return new ModelResponse<VentaDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerVentaPorId");
                return new ModelResponse<VentaDTO> { IsSuccess = false, Message = "Ocurrió un error al consultar la venta." };
            }
        }

        public ModelResponse<VentaDTO> GuardarVenta(long empresaId, VentaDTO venta, string usuario)
        {
            try
            {
                Log.Information("VentaService.GuardarVenta para {Usuario}", usuario);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (venta == null || venta.SucursalId <= 0)
                {
                    throw new ArgumentException("La sucursal es requerida.");
                }

                if (venta.UsuarioId <= 0)
                {
                    throw new ArgumentException("El cajero es requerido.");
                }

                if (string.IsNullOrWhiteSpace(venta.MetodoPago))
                {
                    throw new ArgumentException("El método de pago es requerido.");
                }

                if (venta.Detalle == null || venta.Detalle.Count == 0)
                {
                    throw new ArgumentException("La venta debe incluir al menos un producto.");
                }

                return _dbWrapper.GuardarVenta(empresaId, venta, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarVenta");
                return new ModelResponse<VentaDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarVenta");
                return new ModelResponse<VentaDTO> { IsSuccess = false, Message = "Ocurrió un error al guardar la venta." };
            }
        }
    }
}
