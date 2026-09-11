using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    public class ClienteService
    {
        private readonly DbWrapper _dbWrapper;

        public ClienteService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<List<Cliente>> ObtenerClientes(long empresaId)
        {
            try
            {
                Log.Information("ClienteService.ObtenerClientes para empresa {EmpresaId}", empresaId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                return _dbWrapper.ObtenerClientes(empresaId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerClientes");
                return new ModelResponse<List<Cliente>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerClientes");
                return new ModelResponse<List<Cliente>> { IsSuccess = false, Message = "Ocurrió un error al obtener los clientes." };
            }
        }

        public ModelResponse<Cliente> ObtenerClientePorId(long empresaId, long clienteId)
        {
            try
            {
                Log.Information("ClienteService.ObtenerClientePorId {ClienteId}", clienteId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (clienteId <= 0)
                {
                    throw new ArgumentException("El identificador de Cliente es inválido.");
                }

                return _dbWrapper.ObtenerClientePorId(empresaId, clienteId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerClientePorId");
                return new ModelResponse<Cliente> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerClientePorId");
                return new ModelResponse<Cliente> { IsSuccess = false, Message = "Ocurrió un error al consultar el cliente." };
            }
        }

        public ModelResponse<Cliente> GuardarOActualizarCliente(long empresaId, Cliente cliente, string usuario)
        {
            try
            {
                Log.Information("ClienteService.GuardarOActualizarCliente para {Usuario}", usuario);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (cliente == null || string.IsNullOrWhiteSpace(cliente.Nombre))
                {
                    throw new ArgumentException("El Nombre es requerido.");
                }

                return _dbWrapper.GuardarOActualizarCliente(empresaId, cliente, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarOActualizarCliente");
                return new ModelResponse<Cliente> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarOActualizarCliente");
                return new ModelResponse<Cliente> { IsSuccess = false, Message = "Ocurrió un error al guardar el cliente." };
            }
        }

        public ModelResponse EliminarCliente(long empresaId, long clienteId, string usuario)
        {
            try
            {
                Log.Information("ClienteService.EliminarCliente {ClienteId}", clienteId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (clienteId <= 0)
                {
                    throw new ArgumentException("El identificador de Cliente es inválido.");
                }

                return _dbWrapper.EliminarCliente(empresaId, clienteId, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en EliminarCliente");
                return new ModelResponse { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en EliminarCliente");
                return new ModelResponse { IsSuccess = false, Message = "Ocurrió un error al desactivar el cliente." };
            }
        }
    }
}
