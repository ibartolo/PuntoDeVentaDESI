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
        public ModelResponse<List<ProductoDTO>> ObtenerProductos(long empresaId)
        {
            var mr = new ModelResponse<List<ProductoDTO>>();
            try
            {
                var productos = GetObjects(
                    "sp_Producto_Listar",
                    new Func<IDataReader, ProductoDTO>(r => LlenarEntidad<ProductoDTO>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId) });

                mr.IsSuccess = true;
                mr.Response = productos.ToList();
                mr.Message = "Productos obtenidos correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener productos para empresa {EmpresaId}", empresaId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener los productos.";
            }

            return mr;
        }

        public ModelResponse<ProductoDTO> ObtenerProductoPorId(long empresaId, long productoId)
        {
            var mr = new ModelResponse<ProductoDTO>();
            try
            {
                var producto = GetObject(
                    "sp_Producto_Obtener",
                    new Func<IDataReader, ProductoDTO>(r => LlenarEntidad<ProductoDTO>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId), new SqlParameter("@ProductoId", productoId) });

                if (producto == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Producto no encontrado.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = producto;
                mr.Message = "Producto obtenido correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener producto {ProductoId}", productoId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al consultar el producto.";
            }

            return mr;
        }

        public ModelResponse<ProductoDTO> ObtenerProductoPorCodigo(long empresaId, string codigo)
        {
            var mr = new ModelResponse<ProductoDTO>();
            try
            {
                var producto = GetObject(
                    "sp_Producto_PorCodigo",
                    new Func<IDataReader, ProductoDTO>(r => LlenarEntidad<ProductoDTO>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId), new SqlParameter("@Codigo", codigo) });

                if (producto == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Producto no encontrado para el código indicado.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = producto;
                mr.Message = "Producto obtenido correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener producto por código {Codigo}", codigo);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al consultar el producto por código.";
            }

            return mr;
        }

        public ModelResponse<ProductoDTO> GuardarOActualizarProducto(long empresaId, Producto producto, string usuario)
        {
            var mr = new ModelResponse<ProductoDTO>();
            try
            {
                if (producto == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "El producto es requerido.";
                    return mr;
                }

                var actor = (usuario ?? producto.CreadoPor ?? producto.ModificadoPor ?? "system").Trim();
                if (actor.Length > 25)
                {
                    actor = actor.Substring(0, 25);
                }

                var pars = new[]
                {
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@ProductoId", producto.Id),
                    new SqlParameter("@CategoriaId", (object)producto.CategoriaId ?? DBNull.Value),
                    new SqlParameter("@MarcaId", (object)producto.MarcaId ?? DBNull.Value),
                    new SqlParameter("@MarcaTexto", (object)producto.MarcaTexto ?? DBNull.Value),
                    new SqlParameter("@Nombre", producto.Nombre),
                    new SqlParameter("@Descripcion", (object)producto.Descripcion ?? DBNull.Value),
                    new SqlParameter("@FotoUrl", (object)producto.FotoUrl ?? DBNull.Value),
                    new SqlParameter("@Codigo", (object)producto.Codigo ?? DBNull.Value),
                    new SqlParameter("@TipoProducto", string.IsNullOrWhiteSpace(producto.TipoProducto) ? "Comprado" : producto.TipoProducto),
                    new SqlParameter("@UnidadMedida", (object)producto.UnidadMedida ?? DBNull.Value),
                    new SqlParameter("@StockMinimo", producto.StockMinimo),
                    new SqlParameter("@Actor", actor)
                };

                var id = ExecuteScalar("sp_Producto_Guardar", CommandType.StoredProcedure, pars);
                producto.Id = id == null || id == DBNull.Value ? 0L : Convert.ToInt64(id);

                if (producto.Id <= 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No se pudo guardar el producto.";
                    return mr;
                }

                return ObtenerProductoPorId(empresaId, producto.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar producto");
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar el producto.";
            }

            return mr;
        }

        public ModelResponse EliminarProducto(long empresaId, long productoId, string usuario)
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
                    new SqlParameter("@ProductoId", productoId),
                    new SqlParameter("@Actor", actor)
                };

                var affected = ExecuteScalar("sp_Producto_EliminarLogico", CommandType.StoredProcedure, pars);
                if (affected != null && Convert.ToInt32(affected) == 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "Producto no encontrado o ya inactivo.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Message = "Producto desactivado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar producto {ProductoId}", productoId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al desactivar el producto.";
            }

            return mr;
        }
    }
}
