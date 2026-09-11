using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVenta.Entities.Contracts;
using PuntoDeVenta.Entities.Models;
using PuntoDeVenta.WebApi.Models;
using PuntoDeVenta.WebApi.Services;

namespace PuntoDeVenta.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/marcas")]
    public class MarcasController : BaseApiController
    {
        private readonly MarcaService _marcaService;

        public MarcasController()
        {
            _marcaService = new MarcaService();
        }

        [HttpGet]
        [Route("")]
        public ModelResponse<List<Marca>> Get()
        {
            return _marcaService.Listar(EmpresaIdAutenticada);
        }

        [HttpGet]
        [Route("{id:long}")]
        public ModelResponse<Marca> Get(long id)
        {
            return _marcaService.Consultar(EmpresaIdAutenticada, id);
        }

        [HttpPost]
        [Route("")]
        public ModelResponse<Marca> Post([FromBody] MarcaUpsertRequest request)
        {
            if (request == null)
            {
                return new ModelResponse<Marca>
                {
                    Success = false,
                    Message = "El cuerpo de la solicitud es requerido.",
                    Errors = new List<string> { "El cuerpo de la solicitud es requerido." }
                };
            }

            return _marcaService.Crear(
                EmpresaIdAutenticada,
                SubjectAutenticado,
                request.Nombre,
                request.Descripcion);
        }

        [HttpPut]
        [Route("{id:long}")]
        public ModelResponse<Marca> Put(long id, [FromBody] MarcaUpsertRequest request)
        {
            if (request == null)
            {
                return new ModelResponse<Marca>
                {
                    Success = false,
                    Message = "El cuerpo de la solicitud es requerido.",
                    Errors = new List<string> { "El cuerpo de la solicitud es requerido." }
                };
            }

            return _marcaService.Actualizar(
                EmpresaIdAutenticada,
                id,
                SubjectAutenticado,
                request.Nombre,
                request.Descripcion);
        }

        [HttpDelete]
        [Route("{id:long}")]
        public ModelResponse Delete(long id)
        {
            return _marcaService.EliminarLogico(EmpresaIdAutenticada, id, SubjectAutenticado);
        }
    }
}
