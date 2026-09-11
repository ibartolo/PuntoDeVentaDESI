using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using PuntoDeVentaEntities.Reportes;
using PuntoDeVentaEntities.Seguridad;
using Serilog;

namespace PuntoDeVentaWebApi.DAL
{
    /// <summary>
    /// Acceso a datos de los reportes (ventas por periodo/sucursal/cajero, utilidad y
    /// productos más vendidos). Capa de solo lectura: únicamente ejecuta SPs de consulta.
    /// </summary>
    public partial class DbWrapper
    {
        /// <summary>Ventas detalladas en un rango de fechas, con filtro opcional por sucursal.</summary>
        public ModelResponse<List<ReporteVentasPeriodoDTO>> ObtenerVentasPorPeriodo(long empresaId, DateTime fechaInicio, DateTime fechaFin, long sucursalId)
        {
            var mr = new ModelResponse<List<ReporteVentasPeriodoDTO>>();
            try
            {
                var datos = GetObjects(
                    "sp_Reporte_VentasPorPeriodo",
                    new Func<IDataReader, ReporteVentasPeriodoDTO>(r => LlenarEntidad<ReporteVentasPeriodoDTO>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@FechaInicio", fechaInicio),
                        new SqlParameter("@FechaFin", fechaFin),
                        new SqlParameter("@SucursalId", sucursalId)
                    });

                mr.IsSuccess = true;
                mr.Response = datos.ToList();
                mr.Message = "Reporte de ventas por periodo generado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener el reporte de ventas por periodo para empresa {EmpresaId}", empresaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al generar el reporte de ventas por periodo.";
            }

            return mr;
        }

        /// <summary>Totales de venta agrupados por sucursal en un rango de fechas.</summary>
        public ModelResponse<List<ReporteVentasSucursalDTO>> ObtenerVentasPorSucursal(long empresaId, DateTime fechaInicio, DateTime fechaFin)
        {
            var mr = new ModelResponse<List<ReporteVentasSucursalDTO>>();
            try
            {
                var datos = GetObjects(
                    "sp_Reporte_VentasPorSucursal",
                    new Func<IDataReader, ReporteVentasSucursalDTO>(r => LlenarEntidad<ReporteVentasSucursalDTO>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@FechaInicio", fechaInicio),
                        new SqlParameter("@FechaFin", fechaFin)
                    });

                mr.IsSuccess = true;
                mr.Response = datos.ToList();
                mr.Message = "Reporte de ventas por sucursal generado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener el reporte de ventas por sucursal para empresa {EmpresaId}", empresaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al generar el reporte de ventas por sucursal.";
            }

            return mr;
        }

        /// <summary>Totales de venta agrupados por cajero en un rango de fechas.</summary>
        public ModelResponse<List<ReporteVentasCajeroDTO>> ObtenerVentasPorCajero(long empresaId, DateTime fechaInicio, DateTime fechaFin)
        {
            var mr = new ModelResponse<List<ReporteVentasCajeroDTO>>();
            try
            {
                var datos = GetObjects(
                    "sp_Reporte_VentasPorCajero",
                    new Func<IDataReader, ReporteVentasCajeroDTO>(r => LlenarEntidad<ReporteVentasCajeroDTO>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@FechaInicio", fechaInicio),
                        new SqlParameter("@FechaFin", fechaFin)
                    });

                mr.IsSuccess = true;
                mr.Response = datos.ToList();
                mr.Message = "Reporte de ventas por cajero generado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener el reporte de ventas por cajero para empresa {EmpresaId}", empresaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al generar el reporte de ventas por cajero.";
            }

            return mr;
        }

        /// <summary>Utilidad por producto (venta - costo) en un rango de fechas.</summary>
        public ModelResponse<List<ReporteUtilidadDTO>> ObtenerUtilidad(long empresaId, DateTime fechaInicio, DateTime fechaFin)
        {
            var mr = new ModelResponse<List<ReporteUtilidadDTO>>();
            try
            {
                var datos = GetObjects(
                    "sp_Reporte_Utilidad",
                    new Func<IDataReader, ReporteUtilidadDTO>(r => LlenarEntidad<ReporteUtilidadDTO>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@FechaInicio", fechaInicio),
                        new SqlParameter("@FechaFin", fechaFin)
                    });

                mr.IsSuccess = true;
                mr.Response = datos.ToList();
                mr.Message = "Reporte de utilidad generado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener el reporte de utilidad para empresa {EmpresaId}", empresaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al generar el reporte de utilidad.";
            }

            return mr;
        }

        /// <summary>Productos más vendidos (top N) en un rango de fechas.</summary>
        public ModelResponse<List<ReporteMasVendidosDTO>> ObtenerMasVendidos(long empresaId, DateTime fechaInicio, DateTime fechaFin, int top)
        {
            var mr = new ModelResponse<List<ReporteMasVendidosDTO>>();
            try
            {
                var datos = GetObjects(
                    "sp_Reporte_MasVendidos",
                    new Func<IDataReader, ReporteMasVendidosDTO>(r => LlenarEntidad<ReporteMasVendidosDTO>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@FechaInicio", fechaInicio),
                        new SqlParameter("@FechaFin", fechaFin),
                        new SqlParameter("@Top", top)
                    });

                mr.IsSuccess = true;
                mr.Response = datos.ToList();
                mr.Message = "Reporte de productos más vendidos generado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener el reporte de productos más vendidos para empresa {EmpresaId}", empresaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al generar el reporte de productos más vendidos.";
            }

            return mr;
        }
    }
}
