using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Categoria")]
    public class CategoriaController : BaseController
    {
        private readonly CategoriaService _categoriaService;

        public CategoriaController()
        {
            _categoriaService = new CategoriaService();
        }

        [HttpGet, Route("List")]
        public ModelResponse<List<CategoriaDTO>> ObtenerTodasLasCategorias()
        {
            return _categoriaService.ObtenerCategorias(ObtenerEmpresaIdDesdeClaim());
        }

        [HttpGet, Route("PorPadre/{id:long}")]
        public ModelResponse<List<CategoriaDTO>> ObtenerCategoriasPorPadre(long id)
        {
            return _categoriaService.ObtenerCategoriasPorPadre(ObtenerEmpresaIdDesdeClaim(), id);
        }

        [HttpGet, Route("{id:long}")]
        public ModelResponse<Categoria> ObtenerCategoriaPorId(long id)
        {
            return _categoriaService.ObtenerCategoriaPorId(ObtenerEmpresaIdDesdeClaim(), id);
        }

        [HttpPost, Route("Guardar")]
        public ModelResponse<Categoria> GuardarOActualizarCategoria(Categoria categoria)
        {
            return _categoriaService.GuardarOActualizarCategoria(ObtenerEmpresaIdDesdeClaim(), categoria, User.Identity.Name);
        }

        [HttpDelete, Route("Eliminar")]
        public ModelResponse EliminarCategoria(Categoria categoria)
        {
            return _categoriaService.EliminarCategoria(ObtenerEmpresaIdDesdeClaim(), categoria.Id, User.Identity.Name);
        }
    }
}
