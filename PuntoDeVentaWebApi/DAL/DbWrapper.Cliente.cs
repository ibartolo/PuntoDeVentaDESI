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
        public ModelResponse<List<Cliente>> ObtenerClientes(long empresaId)
        {
            var mr = new ModelResponse<List<Cliente>>();
            try
            {
                var clientes = GetObjects(
                    "sp_Cliente_Listar",
                    new Func<IDataReader, Cliente>(r => LlenarEntidad<Cliente>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId) });

                mr.IsSuccess = true;
                mr.Response = clientes.ToList();
                mr.Message = "Clientes obtenidos correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener clientes para empresa {EmpresaId}", empresaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener los clientes.";
            }

            return mr;
        }

        public ModelResponse<Cliente> ObtenerClientePorId(long empresaId, long clienteId)
        {
            var mr = new ModelResponse<Cliente>();
            try
            {
                var cliente = GetObject(
                    "sp_Cliente_Consultar",
                    new Func<IDataReader, Cliente>(r => LlenarEntidad<Cliente>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId), new SqlParameter("@ClienteId", clienteId) });

                if (cliente == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Cliente no encontrado.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = cliente;
                mr.Message = "Cliente obtenido correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener cliente {ClienteId}", clienteId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al consultar el cliente.";
            }

            return mr;
        }

        public ModelResponse<Cliente> GuardarOActualizarCliente(long empresaId, Cliente cliente, string usuario)
        {
            var mr = new ModelResponse<Cliente>();
            try
            {
                if (cliente == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "El cliente es requerido.";
                    return mr;
                }

                var actor = (usuario ?? cliente.CreadoPor ?? cliente.ModificadoPor ?? "system").Trim();
                if (actor.Length > 25)
                {
                    actor = actor.Substring(0, 25);
                }

                if (cliente.Id == 0)
                {
                    var pars = new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@Nombre", cliente.Nombre),
                        new SqlParameter("@Correo", (object)cliente.Correo ?? DBNull.Value),
                        new SqlParameter("@Telefono", (object)cliente.Telefono ?? DBNull.Value),
                        new SqlParameter("@Direccion", (object)cliente.Direccion ?? DBNull.Value),
                        new SqlParameter("@EsPublicoGeneral", cliente.EsPublicoGeneral),
                        new SqlParameter("@Actor", actor)
                    };

                    var id = ExecuteScalar("sp_Cliente_Insertar", CommandType.StoredProcedure, pars);
                    cliente.Id = Convert.ToInt64(id);
                }
                else
                {
                    var pars = new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@ClienteId", cliente.Id),
                        new SqlParameter("@Nombre", cliente.Nombre),
                        new SqlParameter("@Correo", (object)cliente.Correo ?? DBNull.Value),
                        new SqlParameter("@Telefono", (object)cliente.Telefono ?? DBNull.Value),
                        new SqlParameter("@Direccion", (object)cliente.Direccion ?? DBNull.Value),
                        new SqlParameter("@EsPublicoGeneral", cliente.EsPublicoGeneral),
                        new SqlParameter("@Actor", actor)
                    };

                    var affected = ExecuteScalar("sp_Cliente_Actualizar", CommandType.StoredProcedure, pars);
                    if (affected != null && Convert.ToInt32(affected) == 0)
                    {
                        mr.IsSuccess = false;
                        mr.Message = "Cliente no encontrado o inactivo.";
                        return mr;
                    }
                }

                return ObtenerClientePorId(empresaId, cliente.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar cliente");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar el cliente.";
            }

            return mr;
        }

        public ModelResponse EliminarCliente(long empresaId, long clienteId, string usuario)
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
                    new SqlParameter("@ClienteId", clienteId),
                    new SqlParameter("@Actor", actor)
                };

                var affected = ExecuteScalar("sp_Cliente_EliminarLogico", CommandType.StoredProcedure, pars);
                if (affected != null && Convert.ToInt32(affected) == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Cliente no encontrado o ya inactivo.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Message = "Cliente desactivado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar cliente {ClienteId}", clienteId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al desactivar el cliente.";
            }

            return mr;
        }
    }
}
