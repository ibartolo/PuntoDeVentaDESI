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
        public ModelResponse<List<Marca>> ObtenerMarcas(long empresaId)
        {
            var mr = new ModelResponse<List<Marca>>();
            try
            {
                var marcas = GetObjects(
                    "sp_Marca_Listar",
                    new Func<IDataReader, Marca>(r => LlenarEntidad<Marca>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId) });

                mr.IsSuccess = true;
                mr.Response = marcas.ToList();
                mr.Message = "Marcas obtenidas correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener marcas para empresa {EmpresaId}", empresaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener las marcas.";
            }

            return mr;
        }

        public ModelResponse<Marca> ObtenerMarcaPorId(long empresaId, long marcaId)
        {
            var mr = new ModelResponse<Marca>();
            try
            {
                var marca = GetObject(
                    "sp_Marca_Consultar",
                    new Func<IDataReader, Marca>(r => LlenarEntidad<Marca>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId), new SqlParameter("@MarcaId", marcaId) });

                if (marca == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Marca no encontrada.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = marca;
                mr.Message = "Marca obtenida correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener marca {MarcaId}", marcaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al consultar la marca.";
            }

            return mr;
        }

        public ModelResponse<Marca> GuardarOActualizarMarca(long empresaId, Marca marca, string usuario)
        {
            var mr = new ModelResponse<Marca>();
            try
            {
                if (marca == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "La marca es requerida.";
                    return mr;
                }

                var actor = (usuario ?? marca.CreadoPor ?? marca.ModificadoPor ?? "system").Trim();
                if (actor.Length > 25)
                {
                    actor = actor.Substring(0, 25);
                }

                if (marca.Id == 0)
                {
                    var pars = new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@Nombre", marca.Nombre),
                        new SqlParameter("@Descripcion", (object)marca.Descripcion ?? DBNull.Value),
                        new SqlParameter("@Actor", actor)
                    };

                    var id = ExecuteScalar("sp_Marca_Insertar", CommandType.StoredProcedure, pars);
                    marca.Id = Convert.ToInt64(id);
                }
                else
                {
                    var pars = new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@MarcaId", marca.Id),
                        new SqlParameter("@Nombre", marca.Nombre),
                        new SqlParameter("@Descripcion", (object)marca.Descripcion ?? DBNull.Value),
                        new SqlParameter("@Actor", actor)
                    };

                    var affected = ExecuteScalar("sp_Marca_Actualizar", CommandType.StoredProcedure, pars);
                    if (affected != null && Convert.ToInt32(affected) == 0)
                    {
                        mr.IsSuccess = false;
                        mr.Message = "Marca no encontrada o inactiva.";
                        return mr;
                    }
                }

                return ObtenerMarcaPorId(empresaId, marca.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar marca");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar la marca.";
            }

            return mr;
        }

        public ModelResponse EliminarMarca(long empresaId, long marcaId, string usuario)
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
                    new SqlParameter("@MarcaId", marcaId),
                    new SqlParameter("@Actor", actor)
                };

                var affected = ExecuteScalar("sp_Marca_EliminarLogico", CommandType.StoredProcedure, pars);
                if (affected != null && Convert.ToInt32(affected) == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Marca no encontrada o ya inactiva.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Message = "Marca desactivada correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar marca {MarcaId}", marcaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al desactivar la marca.";
            }

            return mr;
        }
    }
}
