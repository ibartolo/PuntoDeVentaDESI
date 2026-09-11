using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Producto")]
    public class ProductoController : BaseController
    {
        private readonly ProductoService _productoService;

        public ProductoController()
        {
            _productoService = new ProductoService();
        }

        [HttpGet, Route("List")]
        public ModelResponse<List<ProductoDTO>> ObtenerTodosLosProductos()
        {
            return _productoService.ObtenerProductos(ObtenerEmpresaIdDesdeClaim());
        }

        [HttpGet, Route("{id:long}")]
        public ModelResponse<ProductoDTO> ObtenerProductoPorId(long id)
        {
            return _productoService.ObtenerProductoPorId(ObtenerEmpresaIdDesdeClaim(), id);
        }

        [HttpGet, Route("PorCodigo/{codigo}")]
        public ModelResponse<ProductoDTO> ObtenerProductoPorCodigo(string codigo)
        {
            return _productoService.ObtenerProductoPorCodigo(ObtenerEmpresaIdDesdeClaim(), codigo);
        }

        [HttpPost, Route("Guardar")]
        public ModelResponse<ProductoDTO> GuardarOActualizarProducto(Producto producto)
        {
            return _productoService.GuardarOActualizarProducto(ObtenerEmpresaIdDesdeClaim(), producto, User.Identity.Name);
        }

        [HttpDelete, Route("Eliminar")]
        public ModelResponse EliminarProducto(Producto producto)
        {
            return _productoService.EliminarProducto(ObtenerEmpresaIdDesdeClaim(), producto.Id, User.Identity.Name);
        }
    }
}
