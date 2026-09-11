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
        public ModelResponse<List<Proveedor>> ObtenerProveedores(long empresaId)
        {
            var mr = new ModelResponse<List<Proveedor>>();
            try
            {
                var proveedores = GetObjects(
                    "sp_Proveedor_Listar",
                    new Func<IDataReader, Proveedor>(r => LlenarEntidad<Proveedor>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId) });

                mr.IsSuccess = true;
                mr.Response = proveedores.ToList();
                mr.Message = "Proveedores obtenidos correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener proveedores para empresa {EmpresaId}", empresaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener los proveedores.";
            }

            return mr;
        }

        public ModelResponse<Proveedor> ObtenerProveedorPorId(long empresaId, long proveedorId)
        {
            var mr = new ModelResponse<Proveedor>();
            try
            {
                var proveedor = GetObject(
                    "sp_Proveedor_Consultar",
                    new Func<IDataReader, Proveedor>(r => LlenarEntidad<Proveedor>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId), new SqlParameter("@ProveedorId", proveedorId) });

                if (proveedor == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Proveedor no encontrado.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = proveedor;
                mr.Message = "Proveedor obtenido correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener proveedor {ProveedorId}", proveedorId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al consultar el proveedor.";
            }

            return mr;
        }

        public ModelResponse<Proveedor> GuardarOActualizarProveedor(long empresaId, Proveedor proveedor, string usuario)
        {
            var mr = new ModelResponse<Proveedor>();
            try
            {
                if (proveedor == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "El proveedor es requerido.";
                    return mr;
                }

                var actor = (usuario ?? proveedor.CreadoPor ?? proveedor.ModificadoPor ?? "system").Trim();
                if (actor.Length > 25)
                {
                    actor = actor.Substring(0, 25);
                }

                if (proveedor.Id == 0)
                {
                    var pars = new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@Nombre", proveedor.Nombre),
                        new SqlParameter("@Contacto", (object)proveedor.Contacto ?? DBNull.Value),
                        new SqlParameter("@Correo", (object)proveedor.Correo ?? DBNull.Value),
                        new SqlParameter("@Telefono", (object)proveedor.Telefono ?? DBNull.Value),
                        new SqlParameter("@Direccion", (object)proveedor.Direccion ?? DBNull.Value),
                        new SqlParameter("@Actor", actor)
                    };

                    var id = ExecuteScalar("sp_Proveedor_Insertar", CommandType.StoredProcedure, pars);
                    proveedor.Id = Convert.ToInt64(id);
                }
                else
                {
                    var pars = new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@ProveedorId", proveedor.Id),
                        new SqlParameter("@Nombre", proveedor.Nombre),
                        new SqlParameter("@Contacto", (object)proveedor.Contacto ?? DBNull.Value),
                        new SqlParameter("@Correo", (object)proveedor.Correo ?? DBNull.Value),
                        new SqlParameter("@Telefono", (object)proveedor.Telefono ?? DBNull.Value),
                        new SqlParameter("@Direccion", (object)proveedor.Direccion ?? DBNull.Value),
                        new SqlParameter("@Actor", actor)
                    };

                    var affected = ExecuteScalar("sp_Proveedor_Actualizar", CommandType.StoredProcedure, pars);
                    if (affected != null && Convert.ToInt32(affected) == 0)
                    {
                        mr.IsSuccess = false;
                        mr.Message = "Proveedor no encontrado o inactivo.";
                        return mr;
                    }
                }

                return ObtenerProveedorPorId(empresaId, proveedor.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar proveedor");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar el proveedor.";
            }

            return mr;
        }

        public ModelResponse EliminarProveedor(long empresaId, long proveedorId, string usuario)
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
                    new SqlParameter("@ProveedorId", proveedorId),
                    new SqlParameter("@Actor", actor)
                };

                var affected = ExecuteScalar("sp_Proveedor_EliminarLogico", CommandType.StoredProcedure, pars);
                if (affected != null && Convert.ToInt32(affected) == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Proveedor no encontrado o ya inactivo.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Message = "Proveedor desactivado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar proveedor {ProveedorId}", proveedorId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al desactivar el proveedor.";
            }

            return mr;
        }
    }
}
