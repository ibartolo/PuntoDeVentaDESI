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
        /// <summary>
        /// Lista las cancelaciones/devoluciones activas de la empresa. Si sucursalId es 0,
        /// devuelve las de todas las sucursales (filtro del SP sp_Cancelacion_Listar).
        /// </summary>
        public ModelResponse<List<CancelacionDTO>> ObtenerCancelaciones(long empresaId, long sucursalId)
        {
            var mr = new ModelResponse<List<CancelacionDTO>>();
            try
            {
                var cancelaciones = GetObjects(
                    "sp_Cancelacion_Listar",
                    new Func<IDataReader, CancelacionDTO>(r => LlenarEntidad<CancelacionDTO>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@SucursalId", sucursalId)
                    });

                mr.IsSuccess = true;
                mr.Response = cancelaciones.ToList();
                mr.Message = "Cancelaciones obtenidas correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener cancelaciones para empresa {EmpresaId}", empresaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener las cancelaciones.";
            }

            return mr;
        }

        /// <summary>
        /// Obtiene una cancelación con su detalle de devolución. El SP devuelve dos result sets
        /// (encabezado y detalle), por lo que se leen de forma secuencial.
        /// </summary>
        public ModelResponse<CancelacionDTO> ObtenerCancelacionPorId(long empresaId, long cancelacionId)
        {
            var mr = new ModelResponse<CancelacionDTO>();
            try
            {
                var connection = GetOpenConnection();
                using (var command = new SqlCommand("sp_Cancelacion_Obtener", connection) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Add(new SqlParameter("@EmpresaId", empresaId));
                    command.Parameters.Add(new SqlParameter("@CancelacionId", cancelacionId));

                    using (var reader = command.ExecuteReader())
                    {
                        CancelacionDTO cancelacion = null;
                        if (reader.Read())
                        {
                            cancelacion = LlenarEntidad<CancelacionDTO>(reader);
                        }

                        if (cancelacion == null)
                        {
                            mr.IsSuccess = false;
                            mr.Message = "Cancelación no encontrada.";
                            return mr;
                        }

                        cancelacion.Detalle = new List<DevolucionDetalleDTO>();
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                cancelacion.Detalle.Add(LlenarEntidad<DevolucionDetalleDTO>(reader));
                            }
                        }

                        mr.IsSuccess = true;
                        mr.Response = cancelacion;
                        mr.Message = "Cancelación obtenida correctamente";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener cancelación {CancelacionId}", cancelacionId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al consultar la cancelación.";
            }

            return mr;
        }

        /// <summary>
        /// Registra una cancelación total o parcial. La atomicidad (encabezado, detalle,
        /// devolución a stock y salida de caja) la garantiza sp_Cancelacion_Guardar.
        /// </summary>
        public ModelResponse<CancelacionDTO> GuardarCancelacion(long empresaId, CancelacionDTO cancelacion, string usuario)
        {
            var mr = new ModelResponse<CancelacionDTO>();
            try
            {
                if (cancelacion == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "La cancelación es requerida.";
                    return mr;
                }

                var actor = (usuario ?? cancelacion.CreadoPor ?? cancelacion.ModificadoPor ?? "system").Trim();
                if (actor.Length > 25)
                {
                    actor = actor.Substring(0, 25);
                }

                var detalleJson = cancelacion.Detalle == null || cancelacion.Detalle.Count == 0
                    ? null
                    : JsonConvert.SerializeObject(cancelacion.Detalle);

                var pars = new[]
                {
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@SucursalId", cancelacion.SucursalId),
                    new SqlParameter("@VentaId", cancelacion.VentaId),
                    new SqlParameter("@Tipo", cancelacion.Tipo),
                    new SqlParameter("@Motivo", (object)cancelacion.Motivo ?? DBNull.Value),
                    new SqlParameter("@UsuarioCancela", (object)cancelacion.UsuarioCancela ?? actor),
                    new SqlParameter("@UsuarioAutoriza", (object)cancelacion.UsuarioAutoriza ?? DBNull.Value),
                    new SqlParameter("@TotalReembolsado", cancelacion.TotalReembolsado),
                    new SqlParameter("@MetodoReembolso", (object)cancelacion.MetodoReembolso ?? DBNull.Value),
                    new SqlParameter("@DetalleJson", (object)detalleJson ?? DBNull.Value),
                    new SqlParameter("@Actor", actor)
                };

                var id = ExecuteScalar("sp_Cancelacion_Guardar", CommandType.StoredProcedure, pars);
                var cancelacionId = id == null || id == DBNull.Value ? 0L : Convert.ToInt64(id);

                if (cancelacionId <= 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No se pudo registrar la cancelación.";
                    return mr;
                }

                return ObtenerCancelacionPorId(empresaId, cancelacionId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar cancelación");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al registrar la cancelación.";
            }

            return mr;
        }
    }
}
