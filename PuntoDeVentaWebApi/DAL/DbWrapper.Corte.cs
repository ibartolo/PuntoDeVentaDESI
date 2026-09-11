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
        public ModelResponse<List<CorteDTO>> ObtenerCortes(long empresaId, long sucursalId)
        {
            var mr = new ModelResponse<List<CorteDTO>>();
            try
            {
                var cortes = GetObjects(
                    "sp_Corte_Listar",
                    new Func<IDataReader, CorteDTO>(r => LlenarEntidad<CorteDTO>(r)),
                    CommandType.StoredProcedure,
                    new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@SucursalId", sucursalId)
                    });

                mr.IsSuccess = true;
                mr.Response = cortes.ToList();
                mr.Message = "Cortes obtenidos correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener cortes");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener los cortes.";
            }

            return mr;
        }

        public ModelResponse<CorteDTO> ObtenerCortePorId(long empresaId, long corteId)
        {
            var mr = new ModelResponse<CorteDTO>();
            try
            {
                var connection = GetOpenConnection();
                using (var command = new SqlCommand("sp_Corte_Obtener", connection) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Add(new SqlParameter("@EmpresaId", empresaId));
                    command.Parameters.Add(new SqlParameter("@CorteId", corteId));

                    using (var reader = command.ExecuteReader())
                    {
                        CorteDTO corte = null;
                        if (reader.Read())
                        {
                            corte = LlenarEntidad<CorteDTO>(reader);
                        }

                        if (corte == null)
                        {
                            mr.IsSuccess = false;
                            mr.Message = "Corte no encontrado.";
                            return mr;
                        }

                        corte.Detalle = new List<CorteDetalleDTO>();
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                corte.Detalle.Add(LlenarEntidad<CorteDetalleDTO>(reader));
                            }
                        }

                        mr.IsSuccess = true;
                        mr.Response = corte;
                        mr.Message = "Corte obtenido correctamente";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener corte {CorteId}", corteId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al consultar el corte.";
            }

            return mr;
        }

        public ModelResponse<CorteDTO> GuardarCorte(long empresaId, CorteDTO corte, string usuario)
        {
            var mr = new ModelResponse<CorteDTO>();
            try
            {
                if (corte == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "El corte es requerido.";
                    return mr;
                }

                var actor = (usuario ?? corte.CreadoPor ?? corte.ModificadoPor ?? "system").Trim();
                if (actor.Length > 25)
                {
                    actor = actor.Substring(0, 25);
                }

                var pars = new[]
                {
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@SucursalId", corte.SucursalId),
                    new SqlParameter("@UsuarioId", corte.UsuarioId),
                    new SqlParameter("@CajaChicaId", corte.CajaChicaId),
                    new SqlParameter("@MontoInicial", corte.MontoInicial),
                    new SqlParameter("@EfectivoContado", corte.EfectivoContado),
                    new SqlParameter("@Observaciones", (object)corte.Observaciones ?? DBNull.Value),
                    new SqlParameter("@Actor", actor)
                };

                var id = ExecuteScalar("sp_Corte_Guardar", CommandType.StoredProcedure, pars);
                var corteId = id == null || id == DBNull.Value ? 0L : Convert.ToInt64(id);

                if (corteId <= 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No se pudo guardar el corte.";
                    return mr;
                }

                return ObtenerCortePorId(empresaId, corteId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar corte");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar el corte.";
            }

            return mr;
        }
    }
}
