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
        public ModelResponse<PrecioDTO> ObtenerPrecioActivo(long empresaId, long productoId)
        {
            var mr = new ModelResponse<PrecioDTO>();
            try
            {
                var precio = GetObject(
                    "sp_Precio_ObtenerActivo",
                    new Func<IDataReader, PrecioDTO>(r => LlenarEntidad<PrecioDTO>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId), new SqlParameter("@ProductoId", productoId) });

                if (precio == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "El producto no tiene un precio activo.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = precio;
                mr.Message = "Precio activo obtenido correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener precio activo del producto {ProductoId}", productoId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al consultar el precio activo.";
            }

            return mr;
        }

        public ModelResponse<List<PrecioDTO>> ObtenerPreciosPorProducto(long empresaId, long productoId)
        {
            var mr = new ModelResponse<List<PrecioDTO>>();
            try
            {
                var precios = GetObjects(
                    "sp_Precio_PorProducto",
                    new Func<IDataReader, PrecioDTO>(r => LlenarEntidad<PrecioDTO>(r)),
                    CommandType.StoredProcedure,
                    new[] { new SqlParameter("@EmpresaId", empresaId), new SqlParameter("@ProductoId", productoId) });

                mr.IsSuccess = true;
                mr.Response = precios.ToList();
                mr.Message = "Historial de precios obtenido correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener el historial de precios del producto {ProductoId}", productoId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al obtener el historial de precios.";
            }

            return mr;
        }

        public ModelResponse<PrecioDTO> GuardarOActualizarPrecio(long empresaId, Precio precio, string usuario)
        {
            var mr = new ModelResponse<PrecioDTO>();
            try
            {
                if (precio == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "El precio es requerido.";
                    return mr;
                }

                var actor = (usuario ?? precio.CreadoPor ?? precio.ModificadoPor ?? "system").Trim();
                if (actor.Length > 25)
                {
                    actor = actor.Substring(0, 25);
                }

                var pars = new[]
                {
                    new SqlParameter("@EmpresaId", empresaId),
                    new SqlParameter("@ProductoId", precio.ProductoId),
                    new SqlParameter("@PrecioCompra", precio.PrecioCompra),
                    new SqlParameter("@PrecioVentaSugerido", precio.PrecioVentaSugerido),
                    new SqlParameter("@PrecioVenta", precio.PrecioVenta),
                    new SqlParameter("@Actor", actor)
                };

                var id = ExecuteScalar("sp_Precio_Guardar", CommandType.StoredProcedure, pars);
                precio.Id = id == null || id == DBNull.Value ? 0L : Convert.ToInt64(id);

                if (precio.Id <= 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No se pudo guardar el precio.";
                    return mr;
                }

                return ObtenerPrecioActivo(empresaId, precio.ProductoId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar precio del producto {ProductoId}", precio == null ? 0 : precio.ProductoId);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al guardar el precio.";
            }

            return mr;
        }
    }
}
