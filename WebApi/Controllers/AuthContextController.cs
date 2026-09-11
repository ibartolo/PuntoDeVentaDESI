using System.Web.Http;
using PuntoDeVenta.Entities.Contracts;
using PuntoDeVenta.WebApi.Models;

namespace PuntoDeVenta.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/auth-context")]
    public class AuthContextController : BaseApiController
    {
        [HttpGet]
        [Route("")]
        public ModelResponse<AuthenticatedContextDto> Get()
        {
            return new ModelResponse<AuthenticatedContextDto>
            {
                Success = true,
                Message = "Contexto autenticado obtenido.",
                Data = new AuthenticatedContextDto
                {
                    Sub = SubjectAutenticado,
                    EmpresaId = EmpresaIdAutenticada
                }
            };
        }
    }
}
