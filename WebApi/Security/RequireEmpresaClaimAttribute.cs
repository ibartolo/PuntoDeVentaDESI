using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace PuntoDeVenta.WebApi.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireEmpresaClaimAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);

            var principal = actionContext?.ControllerContext?.RequestContext?.Principal as ClaimsPrincipal;
            if (principal == null || !principal.Identity.IsAuthenticated)
            {
                return;
            }

            try
            {
                TenantContext.GetEmpresaIdOrThrow(principal);
                TenantContext.GetSubjectOrThrow(principal);
            }
            catch (InvalidOperationException)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    error = "invalid_token",
                    error_description = "Token sin claims mínimos requeridos (sub, empresaId)."
                });
            }
        }
    }
}
