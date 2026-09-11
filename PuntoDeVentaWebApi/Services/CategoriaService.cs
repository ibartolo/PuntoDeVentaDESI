using System;
using System.Collections.Generic;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    public class CategoriaService
    {
        private readonly DbWrapper _dbWrapper;

        public CategoriaService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<List<CategoriaDTO>> ObtenerCategorias(long empresaId)
        {
            try
            {
                Log.Information("CategoriaService.ObtenerCategorias para empresa {EmpresaId}", empresaId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                return _dbWrapper.ObtenerCategorias(empresaId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerCategorias");
                return new ModelResponse<List<CategoriaDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerCategorias");
                return new ModelResponse<List<CategoriaDTO>> { IsSuccess = false, Message = "Ocurrió un error al obtener las categorías." };
            }
        }

        public ModelResponse<List<CategoriaDTO>> ObtenerCategoriasPorPadre(long empresaId, long categoriaPadreId)
        {
            try
            {
                Log.Information("CategoriaService.ObtenerCategoriasPorPadre {CategoriaPadreId}", categoriaPadreId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (categoriaPadreId <= 0)
                {
                    throw new ArgumentException("El identificador de la categoría padre es inválido.");
                }

                return _dbWrapper.ObtenerCategoriasPorPadre(empresaId, categoriaPadreId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerCategoriasPorPadre");
                return new ModelResponse<List<CategoriaDTO>> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerCategoriasPorPadre");
                return new ModelResponse<List<CategoriaDTO>> { IsSuccess = false, Message = "Ocurrió un error al obtener las subcategorías." };
            }
        }

        public ModelResponse<Categoria> ObtenerCategoriaPorId(long empresaId, long categoriaId)
        {
            try
            {
                Log.Information("CategoriaService.ObtenerCategoriaPorId {CategoriaId}", categoriaId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (categoriaId <= 0)
                {
                    throw new ArgumentException("El identificador de Categoría es inválido.");
                }

                return _dbWrapper.ObtenerCategoriaPorId(empresaId, categoriaId);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en ObtenerCategoriaPorId");
                return new ModelResponse<Categoria> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ObtenerCategoriaPorId");
                return new ModelResponse<Categoria> { IsSuccess = false, Message = "Ocurrió un error al consultar la categoría." };
            }
        }

        public ModelResponse<Categoria> GuardarOActualizarCategoria(long empresaId, Categoria categoria, string usuario)
        {
            try
            {
                Log.Information("CategoriaService.GuardarOActualizarCategoria para {Usuario}", usuario);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
                {
                    throw new ArgumentException("El Nombre es requerido.");
                }

                return _dbWrapper.GuardarOActualizarCategoria(empresaId, categoria, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarOActualizarCategoria");
                return new ModelResponse<Categoria> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarOActualizarCategoria");
                return new ModelResponse<Categoria> { IsSuccess = false, Message = "Ocurrió un error al guardar la categoría." };
            }
        }

        public ModelResponse EliminarCategoria(long empresaId, long categoriaId, string usuario)
        {
            try
            {
                Log.Information("CategoriaService.EliminarCategoria {CategoriaId}", categoriaId);
                if (empresaId <= 0)
                {
                    throw new ArgumentException("No se pudo resolver la empresa autenticada.");
                }

                if (categoriaId <= 0)
                {
                    throw new ArgumentException("El identificador de Categoría es inválido.");
                }

                return _dbWrapper.EliminarCategoria(empresaId, categoriaId, usuario);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en EliminarCategoria");
                return new ModelResponse { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en EliminarCategoria");
                return new ModelResponse { IsSuccess = false, Message = "Ocurrió un error al desactivar la categoría." };
            }
        }
    }
}
