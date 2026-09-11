using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using PuntoDeVentaEntities.Caja;
using PuntoDeVentaEntities.Seguridad;
using Serilog;

namespace PuntoDeVentaWebApi.DAL
{
    public partial class DbWrapper
    {
        /// <summary>Obtiene la caja chica abierta de un usuario en una sucursal (null si no existe).</summary>
        public ModelResponse<CajaChicaDTO> ObtenerCajaChicaAbierta(long empresaId, long sucursalId, long usuarioId)
        {
            var mr = new ModelResponse<CajaChicaDTO>();
            try
            {
                var caja = GetObject(
                    "sp_CajaChica_ObtenerAbierta",
                    new Func<IDataReader, CajaChicaDTO>(r => LlenarEntidad<CajaChicaDTO>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@SucursalId", sucursalId),
                        new SqlParameter("@UsuarioId", usuarioId)
                    });

                mr.IsSuccess = true;
                mr.Response = caja;
                mr.Message = caja == null
                    ? "No hay caja chica abierta para el usuario y la sucursal."
                    : "Caja chica abierta obtenida correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener caja chica abierta para sucursal {SucursalId}", sucursalId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al consultar la caja chica abierta.";
            }

            return mr;
        }

        /// <summary>Abre una caja chica; falla si ya existe una abierta para usuario+sucursal.</summary>
        public ModelResponse<CajaChicaDTO> AbrirCajaChica(long empresaId, long sucursalId, long usuarioId, decimal montoInicial, string usuario)
        {
            var mr = new ModelResponse<CajaChicaDTO>();
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
                    new SqlParameter("@UsuarioId", usuarioId),
                    new SqlParameter("@MontoInicial", montoInicial),
                    new SqlParameter("@Actor", actor)
                };

                var id = ExecuteScalar("sp_CajaChica_Abrir", CommandType.StoredProcedure, pars);
                var cajaChicaId = id == null || id == DBNull.Value ? 0L : Convert.ToInt64(id);

                if (cajaChicaId <= 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No se pudo abrir la caja chica.";
                    return mr;
                }

                return ObtenerCajaChicaAbierta(empresaId, sucursalId, usuarioId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al abrir caja chica para sucursal {SucursalId}", sucursalId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al abrir la caja chica.";
            }

            return mr;
        }

        /// <summary>Cierra la caja chica indicada (cambio de estado Abierta -> Cerrada).</summary>
        public ModelResponse CerrarCajaChica(long empresaId, long cajaChicaId, string usuario)
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
                    new SqlParameter("@CajaChicaId", cajaChicaId),
                    new SqlParameter("@Actor", actor)
                };

                var affected = ExecuteScalar("sp_CajaChica_Cerrar", CommandType.StoredProcedure, pars);
                if (affected != null && Convert.ToInt32(affected) == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Caja chica no encontrada o ya cerrada.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Message = "Caja chica cerrada correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al cerrar caja chica {CajaChicaId}", cajaChicaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al cerrar la caja chica.";
            }

            return mr;
        }

        /// <summary>Registra una salida de caja (la SP valida el límite del 50% de los ingresos).</summary>
        public ModelResponse<SalidaCaja> RegistrarSalidaCaja(long empresaId, SalidaCaja salida, string usuario)
        {
            var mr = new ModelResponse<SalidaCaja>();
            try
            {
                if (salida == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "La salida de caja es requerida.";
                    return mr;
                }

                var actor = (usuario ?? salida.CreadoPor ?? salida.ModificadoPor ?? "system").Trim();
                if (actor.Length > 25)
                {
                    actor = actor.Substring(0, 25);
                }

                var pars = new[]
                {
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@CajaChicaId", salida.CajaChicaId),
                    new SqlParameter("@Monto", salida.Monto),
                    new SqlParameter("@Comentario", (object)salida.Comentario ?? DBNull.Value),
                    new SqlParameter("@Justificada", salida.Justificada),
                    new SqlParameter("@EvidenciaUrl", (object)salida.EvidenciaUrl ?? DBNull.Value),
                    new SqlParameter("@TipoSalida", (object)salida.TipoSalida ?? DBNull.Value),
                    new SqlParameter("@Actor", actor)
                };

                var id = ExecuteScalar("sp_SalidaCaja_Registrar", CommandType.StoredProcedure, pars);
                var salidaId = id == null || id == DBNull.Value ? 0L : Convert.ToInt64(id);

                if (salidaId <= 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No se pudo registrar la salida de caja.";
                    return mr;
                }

                salida.Id = salidaId;
                salida.FechaHora = DateTime.Now;
                salida.Estatus = true;
                mr.IsSuccess = true;
                mr.Response = salida;
                mr.Message = "Salida de caja registrada correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al registrar salida de caja para caja {CajaChicaId}", salida?.CajaChicaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al registrar la salida de caja.";
            }

            return mr;
        }

        /// <summary>Lista las salidas registradas para una caja chica.</summary>
        public ModelResponse<List<SalidaCaja>> ObtenerSalidasCaja(long empresaId, long cajaChicaId)
        {
            var mr = new ModelResponse<List<SalidaCaja>>();
            try
            {
                var salidas = GetObjects(
                    "sp_SalidaCaja_Listar",
                    new Func<IDataReader, SalidaCaja>(r => LlenarEntidad<SalidaCaja>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@CajaChicaId", cajaChicaId)
                    });

                mr.IsSuccess = true;
                mr.Response = salidas.ToList();
                mr.Message = "Salidas de caja obtenidas correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener salidas de caja para caja {CajaChicaId}", cajaChicaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener las salidas de caja.";
            }

            return mr;
        }
    }
}
