using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Reportes;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    /// <summary>
    /// Servicio del backend para los reportes del POS. Valida el tenant, el rango de fechas
    /// y delega en <see cref="DbWrapper"/> la ejecución de los stored procedures.
    /// </summary>
    public class ReporteService
    {
        /// <summary>Top por defecto del reporte de productos más vendidos.</summary>
        private const int TopMasVendidosPorDefecto = 10;

        private readonly DbWrapper _dbWrapper;

        public ReporteService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<List<ReporteVentasPeriodoDTO>> ObtenerVentasPorPeriodo(long empresaId, DateTime fechaInicio, DateTime fechaFin, long sucursalId)
        {
            try
            {
                Log.Information("ReporteService.ObtenerVentasPorPeriodo para empresa {EmpresaId}", empresaId);
                ValidarEmpresa(empresaId);
                ValidarRango(fechaInicio, fechaFin);

                if (sucursalId < 0)
                {
                    throw new ArgumentException("El identificador de Sucursal es inválido.");
                }

                return _dbWrapper.ObtenerVentasPorPeriodo(empresaId, fechaInicio, fechaFin, sucursalId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerVentasPorPeriodo");
                return new ModelResponse<List<ReporteVentasPeriodoDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerVentasPorPeriodo");
                return new ModelResponse<List<ReporteVentasPeriodoDTO>> { IsSuccess = false, Message = "Ocurrió un error al generar el reporte de ventas por periodo." };
            }
        }

        public ModelResponse<List<ReporteVentasSucursalDTO>> ObtenerVentasPorSucursal(long empresaId, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                Log.Information("ReporteService.ObtenerVentasPorSucursal para empresa {EmpresaId}", empresaId);
                ValidarEmpresa(empresaId);
                ValidarRango(fechaInicio, fechaFin);

                return _dbWrapper.ObtenerVentasPorSucursal(empresaId, fechaInicio, fechaFin);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerVentasPorSucursal");
                return new ModelResponse<List<ReporteVentasSucursalDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerVentasPorSucursal");
                return new ModelResponse<List<ReporteVentasSucursalDTO>> { IsSuccess = false, Message = "Ocurrió un error al generar el reporte de ventas por sucursal." };
            }
        }

        public ModelResponse<List<ReporteVentasCajeroDTO>> ObtenerVentasPorCajero(long empresaId, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                Log.Information("ReporteService.ObtenerVentasPorCajero para empresa {EmpresaId}", empresaId);
                ValidarEmpresa(empresaId);
                ValidarRango(fechaInicio, fechaFin);

                return _dbWrapper.ObtenerVentasPorCajero(empresaId, fechaInicio, fechaFin);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerVentasPorCajero");
                return new ModelResponse<List<ReporteVentasCajeroDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerVentasPorCajero");
                return new ModelResponse<List<ReporteVentasCajeroDTO>> { IsSuccess = false, Message = "Ocurrió un error al generar el reporte de ventas por cajero." };
            }
        }

        public ModelResponse<List<ReporteUtilidadDTO>> ObtenerUtilidad(long empresaId, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                Log.Information("ReporteService.ObtenerUtilidad para empresa {EmpresaId}", empresaId);
                ValidarEmpresa(empresaId);
                ValidarRango(fechaInicio, fechaFin);

                return _dbWrapper.ObtenerUtilidad(empresaId, fechaInicio, fechaFin);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerUtilidad");
                return new ModelResponse<List<ReporteUtilidadDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerUtilidad");
                return new ModelResponse<List<ReporteUtilidadDTO>> { IsSuccess = false, Message = "Ocurrió un error al generar el reporte de utilidad." };
            }
        }

        public ModelResponse<List<ReporteMasVendidosDTO>> ObtenerMasVendidos(long empresaId, DateTime fechaInicio, DateTime fechaFin, int top)
        {
            try
            {
                Log.Information("ReporteService.ObtenerMasVendidos para empresa {EmpresaId}", empresaId);
                ValidarEmpresa(empresaId);
                ValidarRango(fechaInicio, fechaFin);

                if (top <= 0)
                {
                    top = TopMasVendidosPorDefecto;
                }

                return _dbWrapper.ObtenerMasVendidos(empresaId, fechaInicio, fechaFin, top);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerMasVendidos");
                return new ModelResponse<List<ReporteMasVendidosDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerMasVendidos");
                return new ModelResponse<List<ReporteMasVendidosDTO>> { IsSuccess = false, Message = "Ocurrió un error al generar el reporte de productos más vendidos." };
            }
        }

        /// <summary>Valida que exista una empresa autenticada resuelta desde el token.</summary>
        private static void ValidarEmpresa(long empresaId)
        {
            if (empresaId <= 0)
            {
                throw new ArgumentException("No se pudo resolver la empresa autenticada.");
            }
        }

        /// <summary>Valida que el rango de fechas sea coherente.</summary>
        private static void ValidarRango(DateTime fechaInicio, DateTime fechaFin)
        {
            if (fechaInicio == default(DateTime))
            {
                throw new ArgumentException("La fecha de inicio es requerida.");
            }

            if (fechaFin == default(DateTime))
            {
                throw new ArgumentException("La fecha de fin es requerida.");
            }

            if (fechaInicio > fechaFin)
            {
                throw new ArgumentException("La fecha de inicio no puede ser mayor que la fecha de fin.");
            }
        }
    }
}
