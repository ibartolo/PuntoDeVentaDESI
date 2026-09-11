using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Compras;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    /// <summary>Servicio del backend para el historial de compras a proveedor.</summary>
    public class CompraService
    {
        private readonly DbWrapper _dbWrapper;

        public CompraService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<List<CompraDTO>> ObtenerCompras(long empresaId, long sucursalId)
        {
            try
            {
                Log.Information("CompraService.ObtenerCompras para empresa {EmpresaId}", empresaId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                return _dbWrapper.ObtenerCompras(empresaId, sucursalId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerCompras");
                return new ModelResponse<List<CompraDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerCompras");
                return new ModelResponse<List<CompraDTO>> { IsSuccess = false, Message = "Ocurrió un error al obtener las compras." };
            }
        }

        public ModelResponse<CompraDTO> ObtenerCompraPorId(long empresaId, long compraId)
        {
            try
            {
                Log.Information("CompraService.ObtenerCompraPorId {CompraId}", compraId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (compraId <= 0)
                {
                    throw new ArgumentException("El identificador de Compra es inválido.");
                }

                return _dbWrapper.ObtenerCompraPorId(empresaId, compraId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerCompraPorId");
                return new ModelResponse<CompraDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerCompraPorId");
                return new ModelResponse<CompraDTO> { IsSuccess = false, Message = "Ocurrió un error al consultar la compra." };
            }
        }

        public ModelResponse<CompraDTO> GuardarCompra(long empresaId, CompraDTO compra, string usuario)
        {
            try
            {
                Log.Information("CompraService.GuardarCompra para {Usuario}", usuario);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (compra == null || compra.SucursalId <= 0)
                {
                    throw new ArgumentException("La sucursal es requerida.");
                }

                if (compra.ProveedorId <= 0)
                {
                    throw new ArgumentException("El proveedor es requerido.");
                }

                if (compra.Detalle == null || compra.Detalle.Count == 0)
                {
                    throw new ArgumentException("La compra debe incluir al menos un producto.");
                }

                return _dbWrapper.GuardarCompra(empresaId, compra, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarCompra");
                return new ModelResponse<CompraDTO> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarCompra");
                return new ModelResponse<CompraDTO> { IsSuccess = false, Message = "Ocurrió un error al guardar la compra." };
            }
        }

        public ModelResponse EliminarCompra(long empresaId, long compraId, string usuario)
        {
            try
            {
                Log.Information("CompraService.EliminarCompra {CompraId}", compraId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (compraId <= 0)
                {
                    throw new ArgumentException("El identificador de Compra es inválido.");
                }

                return _dbWrapper.EliminarCompra(empresaId, compraId, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en EliminarCompra");
                return new ModelResponse { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en EliminarCompra");
                return new ModelResponse { IsSuccess = false, Message = "Ocurrió un error al desactivar la compra." };
            }
        }
    }
}
