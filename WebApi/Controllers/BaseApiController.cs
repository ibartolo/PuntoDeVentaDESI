using System.Security.Claims;
using System.Web.Http;
using PuntoDeVenta.WebApi.Security;

namespace PuntoDeVenta.WebApi.Controllers
{
    [RequireEmpresaClaim]
    public abstract class BaseApiController : ApiController
    {
        protected long EmpresaIdAutenticada
        {
            get { return TenantContext.GetEmpresaIdOrThrow(User as ClaimsPrincipal); }
        }

        protected string SubjectAutenticado
        {
            get { return TenantContext.GetSubjectOrThrow(User as ClaimsPrincipal); }
        }
    }
}
