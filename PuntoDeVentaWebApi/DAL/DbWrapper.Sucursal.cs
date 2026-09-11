using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using Serilog;

namespace PuntoDeVentaWebApi.DAL
{
    public partial class DbWrapper
    {
        public ModelResponse<List<Sucursal>> ObtenerSucursales(long empresaId)
        {
            var mr = new ModelResponse<List<Sucursal>>();
            try
            {
                var sucursales = GetObjects(
                    "sp_Sucursal_Listar",
                    new Func<IDataReader, Sucursal>(r => LlenarEntidad<Sucursal>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId) });

                mr.IsSuccess = true;
                mr.Response = sucursales.ToList();
                mr.Message = "Sucursales obtenidas correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener sucursales para empresa {EmpresaId}", empresaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener las sucursales.";
            }

            return mr;
        }

        public ModelResponse<Sucursal> ObtenerSucursalPorId(long empresaId, long sucursalId)
        {
            var mr = new ModelResponse<Sucursal>();
            try
            {
                var sucursal = GetObject(
                    "sp_Sucursal_Consultar",
                    new Func<IDataReader, Sucursal>(r => LlenarEntidad<Sucursal>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId), new SqlParameter("@SucursalId", sucursalId) });

                if (sucursal == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Sucursal no encontrada.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = sucursal;
                mr.Message = "Sucursal obtenida correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener sucursal {SucursalId}", sucursalId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al consultar la sucursal.";
            }

            return mr;
        }

        public ModelResponse<Sucursal> GuardarOActualizarSucursal(long empresaId, Sucursal sucursal, string usuario)
        {
            var mr = new ModelResponse<Sucursal>();
            try
            {
                if (sucursal == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "La sucursal es requerida.";
                    return mr;
                }

                var actor = (usuario ?? sucursal.CreadoPor ?? sucursal.ModificadoPor ?? "system").Trim();
                if (actor.Length > 25)
                {
                    actor = actor.Substring(0, 25);
                }

                if (sucursal.Id == 0)
                {
                    var pars = new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@Nombre", sucursal.Nombre),
                        new SqlParameter("@Descripcion", (object)sucursal.Descripcion ?? DBNull.Value),
                        new SqlParameter("@Calle", (object)sucursal.Calle ?? DBNull.Value),
                        new SqlParameter("@Ciudad", (object)sucursal.Ciudad ?? DBNull.Value),
                        new SqlParameter("@Colonia", (object)sucursal.Colonia ?? DBNull.Value),
                        new SqlParameter("@CodigoPostal", (object)sucursal.CodigoPostal ?? DBNull.Value),
                        new SqlParameter("@Actor", actor)
                    };

                    var id = ExecuteScalar("sp_Sucursal_Insertar", CommandType.StoredProcedure, pars);
                    sucursal.Id = Convert.ToInt64(id);
                }
                else
                {
                    var pars = new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@SucursalId", sucursal.Id),
                        new SqlParameter("@Nombre", sucursal.Nombre),
                        new SqlParameter("@Descripcion", (object)sucursal.Descripcion ?? DBNull.Value),
                        new SqlParameter("@Calle", (object)sucursal.Calle ?? DBNull.Value),
                        new SqlParameter("@Ciudad", (object)sucursal.Ciudad ?? DBNull.Value),
                        new SqlParameter("@Colonia", (object)sucursal.Colonia ?? DBNull.Value),
                        new SqlParameter("@CodigoPostal", (object)sucursal.CodigoPostal ?? DBNull.Value),
                        new SqlParameter("@Actor", actor)
                    };

                    var affected = ExecuteScalar("sp_Sucursal_Actualizar", CommandType.StoredProcedure, pars);
                    if (affected != null && Convert.ToInt32(affected) == 0)
                    {
                        mr.IsSuccess = false;
                        mr.Message = "Sucursal no encontrada o inactiva.";
                        return mr;
                    }
                }

                return ObtenerSucursalPorId(empresaId, sucursal.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar sucursal");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar la sucursal.";
            }

            return mr;
        }

        public ModelResponse EliminarSucursal(long empresaId, long sucursalId, string usuario)
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
                    new SqlParameter("@SucursalId", sucursalId),
                    new SqlParameter("@Actor", actor)
                };

                var affected = ExecuteScalar("sp_Sucursal_EliminarLogico", CommandType.StoredProcedure, pars);
                if (affected != null && Convert.ToInt32(affected) == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Sucursal no encontrada o ya inactiva.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Message = "Sucursal desactivada correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar sucursal {SucursalId}", sucursalId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al desactivar la sucursal.";
            }

            return mr;
        }
    }
}
