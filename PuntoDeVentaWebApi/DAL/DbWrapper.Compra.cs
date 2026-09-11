using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;
using PuntoDeVentaEntities.Compras;
using PuntoDeVentaEntities.Seguridad;
using Serilog;

namespace PuntoDeVentaWebApi.DAL
{
    public partial class DbWrapper
    {
        public ModelResponse<List<CompraDTO>> ObtenerCompras(long empresaId, long sucursalId)
        {
            var mr = new ModelResponse<List<CompraDTO>>();
            try
            {
                var compras = GetObjects(
                    "sp_Compra_Listar",
                    new Func<IDataReader, CompraDTO>(r => LlenarEntidad<CompraDTO>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@SucursalId", sucursalId)
                    });

                mr.IsSuccess = true;
                mr.Response = compras.ToList();
                mr.Message = "Compras obtenidas correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener compras para empresa {EmpresaId}", empresaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener las compras.";
            }

            return mr;
        }

        public ModelResponse<CompraDTO> ObtenerCompraPorId(long empresaId, long compraId)
        {
            var mr = new ModelResponse<CompraDTO>();
            try
            {
                var connection = GetOpenConnection();
                using (var command = new SqlCommand("sp_Compra_Obtener", connection) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Add(new SqlParameter("@EmpresaId", empresaId));
                    command.Parameters.Add(new SqlParameter("@CompraId", compraId));

                    using (var reader = command.ExecuteReader())
                    {
                        CompraDTO compra = null;
                        if (reader.Read())
                        {
                            compra = LlenarEntidad<CompraDTO>(reader);
                        }

                        if (compra == null)
                        {
                            mr.IsSuccess = false;
                            mr.Message = "Compra no encontrada.";
                            return mr;
                        }

                        compra.Detalle = new List<CompraDetalleDTO>();
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                compra.Detalle.Add(LlenarEntidad<CompraDetalleDTO>(reader));
                            }
                        }

                        mr.IsSuccess = true;
                        mr.Response = compra;
                        mr.Message = "Compra obtenida correctamente";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener compra {CompraId}", compraId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al consultar la compra.";
            }

            return mr;
        }

        public ModelResponse<CompraDTO> GuardarCompra(long empresaId, CompraDTO compra, string usuario)
        {
            var mr = new ModelResponse<CompraDTO>();
            try
            {
                if (compra == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "La compra es requerida.";
                    return mr;
                }

                var actor = (usuario ?? compra.CreadoPor ?? compra.ModificadoPor ?? "system").Trim();
                if (actor.Length > 25)
                {
                    actor = actor.Substring(0, 25);
                }

                var detalleJson = compra.Detalle == null || compra.Detalle.Count == 0
                    ? null
                    : JsonConvert.SerializeObject(compra.Detalle);

                var pars = new[]
                {
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@SucursalId", compra.SucursalId),
                    new SqlParameter("@ProveedorId", compra.ProveedorId),
                    new SqlParameter("@Folio", (object)compra.Folio ?? DBNull.Value),
                    new SqlParameter("@FechaCompra", compra.FechaCompra == default(DateTime) ? (object)DBNull.Value : compra.FechaCompra),
                    new SqlParameter("@Subtotal", compra.Subtotal),
                    new SqlParameter("@Impuesto", compra.Impuesto),
                    new SqlParameter("@Total", compra.Total),
                    new SqlParameter("@Observaciones", (object)compra.Observaciones ?? DBNull.Value),
                    new SqlParameter("@DetalleJson", (object)detalleJson ?? DBNull.Value),
                    new SqlParameter("@Actor", actor)
                };

                var id = ExecuteScalar("sp_Compra_Guardar", CommandType.StoredProcedure, pars);
                var compraId = id == null || id == DBNull.Value ? 0L : Convert.ToInt64(id);

                if (compraId <= 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No se pudo guardar la compra.";
                    return mr;
                }

                return ObtenerCompraPorId(empresaId, compraId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar compra");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar la compra.";
            }

            return mr;
        }

        public ModelResponse EliminarCompra(long empresaId, long compraId, string usuario)
        {
            var mr = new ModelResponse();
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
                    new SqlParameter("@CompraId", compraId),
                    new SqlParameter("@Actor", actor)
                };

                var affected = ExecuteScalar("sp_Compra_EliminarLogico", CommandType.StoredProcedure, pars);
                if (affected != null && Convert.ToInt32(affected) == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Compra no encontrada o ya inactiva.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Message = "Compra desactivada correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar compra {CompraId}", compraId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al desactivar la compra.";
            }

            return mr;
        }
    }
}
