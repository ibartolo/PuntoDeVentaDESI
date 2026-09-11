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
        public ModelResponse<List<CategoriaDTO>> ObtenerCategorias(long empresaId)
        {
            var mr = new ModelResponse<List<CategoriaDTO>>();
            try
            {
                var categorias = GetObjects(
                    "sp_Categoria_Listar",
                    new Func<IDataReader, CategoriaDTO>(r => LlenarEntidad<CategoriaDTO>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId) });

                mr.IsSuccess = true;
                mr.Response = categorias.ToList();
                mr.Message = "Categorías obtenidas correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener categorías para empresa {EmpresaId}", empresaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener las categorías.";
            }

            return mr;
        }

        public ModelResponse<List<CategoriaDTO>> ObtenerCategoriasPorPadre(long empresaId, long categoriaPadreId)
        {
            var mr = new ModelResponse<List<CategoriaDTO>>();
            try
            {
                var categorias = GetObjects(
                    "sp_Categoria_ListarPorPadre",
                    new Func<IDataReader, CategoriaDTO>(r => LlenarEntidad<CategoriaDTO>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId), new SqlParameter("@CategoriaPadreId", categoriaPadreId) });

                mr.IsSuccess = true;
                mr.Response = categorias.ToList();
                mr.Message = "Subcategorías obtenidas correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener subcategorías de {CategoriaPadreId}", categoriaPadreId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener las subcategorías.";
            }

            return mr;
        }

        public ModelResponse<Categoria> ObtenerCategoriaPorId(long empresaId, long categoriaId)
        {
            var mr = new ModelResponse<Categoria>();
            try
            {
                var categoria = GetObject(
                    "sp_Categoria_Consultar",
                    new Func<IDataReader, Categoria>(r => LlenarEntidad<Categoria>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId), new SqlParameter("@CategoriaId", categoriaId) });

                if (categoria == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Categoría no encontrada.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = categoria;
                mr.Message = "Categoría obtenida correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener categoría {CategoriaId}", categoriaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al consultar la categoría.";
            }

            return mr;
        }

        public ModelResponse<Categoria> GuardarOActualizarCategoria(long empresaId, Categoria categoria, string usuario)
        {
            var mr = new ModelResponse<Categoria>();
            try
            {
                if (categoria == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "La categoría es requerida.";
                    return mr;
                }

                var actor = (usuario ?? categoria.CreadoPor ?? categoria.ModificadoPor ?? "system").Trim();
                if (actor.Length > 25)
                {
                    actor = actor.Substring(0, 25);
                }

                var padre = (object)categoria.CategoriaPadreId ?? DBNull.Value;

                if (categoria.Id == 0)
                {
                    var pars = new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@CategoriaPadreId", padre),
                        new SqlParameter("@Nombre", categoria.Nombre),
                        new SqlParameter("@Descripcion", (object)categoria.Descripcion ?? DBNull.Value),
                        new SqlParameter("@Area", (object)categoria.Area ?? DBNull.Value),
                        new SqlParameter("@Orden", categoria.Orden),
                        new SqlParameter("@Actor", actor)
                    };

                    var id = ExecuteScalar("sp_Categoria_Insertar", CommandType.StoredProcedure, pars);
                    categoria.Id = Convert.ToInt64(id);
                }
                else
                {
                    var pars = new[]
                    {
                        new SqlParameter("@EmpresaId", empresaId),
                        new SqlParameter("@CategoriaId", categoria.Id),
                        new SqlParameter("@CategoriaPadreId", padre),
                        new SqlParameter("@Nombre", categoria.Nombre),
                        new SqlParameter("@Descripcion", (object)categoria.Descripcion ?? DBNull.Value),
                        new SqlParameter("@Area", (object)categoria.Area ?? DBNull.Value),
                        new SqlParameter("@Orden", categoria.Orden),
                        new SqlParameter("@Actor", actor)
                    };

                    var affected = ExecuteScalar("sp_Categoria_Actualizar", CommandType.StoredProcedure, pars);
                    if (affected != null && Convert.ToInt32(affected) == 0)
                    {
                        mr.IsSuccess = false;
                        mr.Message = "Categoría no encontrada o inactiva.";
                        return mr;
                    }
                }

                return ObtenerCategoriaPorId(empresaId, categoria.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar categoría");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar la categoría.";
            }

            return mr;
        }

        public ModelResponse EliminarCategoria(long empresaId, long categoriaId, string usuario)
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
                    new SqlParameter("@CategoriaId", categoriaId),
                    new SqlParameter("@Actor", actor)
                };

                var affected = ExecuteScalar("sp_Categoria_EliminarLogico", CommandType.StoredProcedure, pars);
                if (affected != null && Convert.ToInt32(affected) == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Categoría no encontrada o ya inactiva.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Message = "Categoría desactivada correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar categoría {CategoriaId}", categoriaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al desactivar la categoría.";
            }

            return mr;
        }
    }
}
