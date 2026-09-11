using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Marca")]
    public class MarcaController : BaseController
    {
        private readonly MarcaService _marcaService;

        public MarcaController()
        {
            _marcaService = new MarcaService();
        }

        [HttpGet, Route("List")]
        public ModelResponse<List<Marca>> ObtenerTodasLasMarcas()
        {
            return _marcaService.ObtenerMarcas(ObtenerEmpresaIdDesdeClaim());
        }

        [HttpGet, Route("{id:long}")]
        public ModelResponse<Marca> ObtenerMarcaPorId(long id)
        {
            return _marcaService.ObtenerMarcaPorId(ObtenerEmpresaIdDesdeClaim(), id);
        }

        [HttpPost, Route("Guardar")]
        public ModelResponse<Marca> GuardarOActualizarMarca(Marca marca)
        {
            return _marcaService.GuardarOActualizarMarca(ObtenerEmpresaIdDesdeClaim(), marca, User.Identity.Name);
        }

        [HttpDelete, Route("Eliminar")]
        public ModelResponse EliminarMarca(Marca marca)
        {
            return _marcaService.EliminarMarca(ObtenerEmpresaIdDesdeClaim(), marca.Id, User.Identity.Name);
        }
    }
}
