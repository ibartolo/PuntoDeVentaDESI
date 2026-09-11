using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaEntities.Ventas;
using Serilog;

namespace PuntoDeVentaWebApi.DAL
{
    public partial class DbWrapper
    {
        public ModelResponse<List<VentaDTO>> ObtenerVentas(long empresaId, long sucursalId)
        {
            var mr = new ModelResponse<List<VentaDTO>>();
            try
            {
                var ventas = GetObjects(
                    "sp_Venta_Listar",
                    new Func<IDataReader, VentaDTO>(r => LlenarEntidad<VentaDTO>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@SucursalId", sucursalId)
                    });

                mr.IsSuccess = true;
                mr.Response = ventas.ToList();
                mr.Message = "Ventas obtenidas correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener ventas");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener las ventas.";
            }

            return mr;
        }

        public ModelResponse<VentaDTO> ObtenerVentaPorId(long empresaId, long ventaId)
        {
            var mr = new ModelResponse<VentaDTO>();
            try
            {
                var connection = GetOpenConnection();
                using (var command = new SqlCommand("sp_Venta_Obtener", connection) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Add(new SqlParameter("@EmpresaId", empresaId));
                    command.Parameters.Add(new SqlParameter("@VentaId", ventaId));

                    using (var reader = command.ExecuteReader())
                    {
                        VentaDTO venta = null;
                        if (reader.Read())
                        {
                            venta = LlenarEntidad<VentaDTO>(reader);
                        }

                        if (venta == null)
                        {
                            mr.IsSuccess = false;
                            mr.Message = "Venta no encontrada.";
                            return mr;
                        }

                        venta.Detalle = new List<VentaDetalleDTO>();
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                venta.Detalle.Add(LlenarEntidad<VentaDetalleDTO>(reader));
                            }
                        }

                        mr.IsSuccess = true;
                        mr.Response = venta;
                        mr.Message = "Venta obtenida correctamente";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener venta {VentaId}", ventaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al consultar la venta.";
            }

            return mr;
        }

        public ModelResponse<VentaDTO> GuardarVenta(long empresaId, VentaDTO venta, string usuario)
        {
            var mr = new ModelResponse<VentaDTO>();
            try
            {
                if (venta == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "La venta es requerida.";
                    return mr;
                }

                var actor = (usuario ?? venta.CreadoPor ?? venta.ModificadoPor ?? "system").Trim();
                if (actor.Length > 25)
                {
                    actor = actor.Substring(0, 25);
                }

                var detalleJson = venta.Detalle == null || venta.Detalle.Count == 0
                    ? null
                    : JsonConvert.SerializeObject(venta.Detalle);

                var pars = new[]
                {
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@SucursalId", venta.SucursalId),
                    new SqlParameter("@UsuarioId", venta.UsuarioId),
                    new SqlParameter("@ClienteId", (object)venta.ClienteId ?? DBNull.Value),
                    new SqlParameter("@CajaChicaId", (object)venta.CajaChicaId ?? DBNull.Value),
                    new SqlParameter("@Folio", (object)venta.Folio ?? DBNull.Value),
                    new SqlParameter("@MetodoPago", venta.MetodoPago),
                    new SqlParameter("@Subtotal", venta.Subtotal),
                    new SqlParameter("@Impuesto", venta.Impuesto),
                    new SqlParameter("@Total", venta.Total),
                    new SqlParameter("@DetalleJson", (object)detalleJson ?? DBNull.Value),
                    new SqlParameter("@Actor", actor)
                };

                var id = ExecuteScalar("sp_Venta_Guardar", CommandType.StoredProcedure, pars);
                var ventaId = id == null || id == DBNull.Value ? 0L : Convert.ToInt64(id);

                if (ventaId <= 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No se pudo guardar la venta.";
                    return mr;
                }

                return ObtenerVentaPorId(empresaId, ventaId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar venta");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar la venta.";
            }

            return mr;
        }
    }
}
