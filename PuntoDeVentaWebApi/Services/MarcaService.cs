using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    public class MarcaService
    {
        private readonly DbWrapper _dbWrapper;

        public MarcaService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<List<Marca>> ObtenerMarcas(long empresaId)
        {
            try
            {
                Log.Information("MarcaService.ObtenerMarcas para empresa {EmpresaId}", empresaId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                return _dbWrapper.ObtenerMarcas(empresaId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerMarcas");
                return new ModelResponse<List<Marca>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerMarcas");
                return new ModelResponse<List<Marca>> { IsSuccess = false, Message = "Ocurrió un error al obtener las marcas." };
            }
        }

        public ModelResponse<Marca> ObtenerMarcaPorId(long empresaId, long marcaId)
        {
            try
            {
                Log.Information("MarcaService.ObtenerMarcaPorId {MarcaId}", marcaId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (marcaId <= 0)
                {
                    throw new ArgumentException("El identificador de Marca es inválido.");
                }

                return _dbWrapper.ObtenerMarcaPorId(empresaId, marcaId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerMarcaPorId");
                return new ModelResponse<Marca> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerMarcaPorId");
                return new ModelResponse<Marca> { IsSuccess = false, Message = "Ocurrió un error al consultar la marca." };
            }
        }

        public ModelResponse<Marca> GuardarOActualizarMarca(long empresaId, Marca marca, string usuario)
        {
            try
            {
                Log.Information("MarcaService.GuardarOActualizarMarca para {Usuario}", usuario);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (marca == null || string.IsNullOrWhiteSpace(marca.Nombre))
                {
                    throw new ArgumentException("El Nombre es requerido.");
                }

                return _dbWrapper.GuardarOActualizarMarca(empresaId, marca, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarOActualizarMarca");
                return new ModelResponse<Marca> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarOActualizarMarca");
                return new ModelResponse<Marca> { IsSuccess = false, Message = "Ocurrió un error al guardar la marca." };
            }
        }

        public ModelResponse EliminarMarca(long empresaId, long marcaId, string usuario)
        {
            try
            {
                Log.Information("MarcaService.EliminarMarca {MarcaId}", marcaId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (marcaId <= 0)
                {
                    throw new ArgumentException("El identificador de Marca es inválido.");
                }

                return _dbWrapper.EliminarMarca(empresaId, marcaId, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en EliminarMarca");
                return new ModelResponse { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en EliminarMarca");
                return new ModelResponse { IsSuccess = false, Message = "Ocurrió un error al desactivar la marca." };
            }
        }
    }
}
