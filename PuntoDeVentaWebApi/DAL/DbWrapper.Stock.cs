using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using PuntoDeVentaEntities.Inventario;
using PuntoDeVentaEntities.Seguridad;
using Serilog;

namespace PuntoDeVentaWebApi.DAL
{
    public partial class DbWrapper
    {
        public ModelResponse<List<StockDTO>> ObtenerStockPorSucursal(long empresaId, long sucursalId)
        {
            var mr = new ModelResponse<List<StockDTO>>();
            try
            {
                var stock = GetObjects(
                    "sp_Stock_ListarPorSucursal",
                    new Func<IDataReader, StockDTO>(r => LlenarEntidad<StockDTO>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@SucursalId", sucursalId)
                    });

                mr.IsSuccess = true;
                mr.Response = stock.ToList();
                mr.Message = "Stock obtenido correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener stock de la sucursal {SucursalId}", sucursalId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener el stock.";
            }

            return mr;
        }

        public ModelResponse<StockDTO> ObtenerStock(long empresaId, long sucursalId, long productoId)
        {
            var mr = new ModelResponse<StockDTO>();
            try
            {
                var stock = GetObject(
                    "sp_Stock_Obtener",
                    new Func<IDataReader, StockDTO>(r => LlenarEntidad<StockDTO>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@SucursalId", sucursalId),
                        new SqlParameter("@ProductoId", productoId)
                    });

                if (stock == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No hay existencia registrada para el producto en la sucursal.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = stock;
                mr.Message = "Stock obtenido correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener stock del producto {ProductoId}", productoId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al consultar el stock.";
            }

            return mr;
        }

        public ModelResponse<List<StockMovimientoDTO>> ObtenerMovimientos(long empresaId, long sucursalId, long productoId)
        {
            var mr = new ModelResponse<List<StockMovimientoDTO>>();
            try
            {
                var movimientos = GetObjects(
                    "sp_StockMovimiento_Listar",
                    new Func<IDataReader, StockMovimientoDTO>(r => LlenarEntidad<StockMovimientoDTO>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@SucursalId", sucursalId),
                        new SqlParameter("@ProductoId", productoId)
                    });

                mr.IsSuccess = true;
                mr.Response = movimientos.ToList();
                mr.Message = "Movimientos obtenidos correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener movimientos de stock");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener los movimientos de stock.";
            }

            return mr;
        }

        public ModelResponse<decimal> RegistrarMovimientoStock(long empresaId, long sucursalId, long productoId,
            string tipoMovimiento, decimal cantidad, string motivo, long? referenciaId, string usuario)
        {
            var mr = new ModelResponse<decimal>();
            try
            {
                var actor = (usuario ?? "system").Trim();
                if (actor.Length > 25)
                {
                    actor = actor.Substring(0, 25);
                }

                var pars = new[]
                {
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@SucursalId", sucursalId),
                    new SqlParameter("@ProductoId", productoId),
                    new SqlParameter("@TipoMovimiento", tipoMovimiento),
                    new SqlParameter("@Cantidad", cantidad),
                    new SqlParameter("@Motivo", (object)motivo ?? DBNull.Value),
                    new SqlParameter("@ReferenciaId", (object)referenciaId ?? DBNull.Value),
                    new SqlParameter("@Actor", actor)
                };

                var result = ExecuteScalar("sp_Stock_Movimiento", CommandType.StoredProcedure, pars);
                var existenciaNueva = result == null || result == DBNull.Value ? 0m : Convert.ToDecimal(result);

                mr.IsSuccess = true;
                mr.Response = existenciaNueva;
                mr.Message = "Movimiento de stock registrado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al registrar movimiento de stock");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al registrar el movimiento de stock.";
            }

            return mr;
        }
    }
}
