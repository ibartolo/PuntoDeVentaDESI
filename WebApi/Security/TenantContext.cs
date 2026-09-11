using System;
using System.Linq;
using System.Security.Claims;

namespace PuntoDeVenta.WebApi.Security
{
    public static class TenantContext
    {
        public static long GetEmpresaIdOrThrow(ClaimsPrincipal principal)
        {
            var claimValue = principal?.Claims?.FirstOrDefault(c => c.Type == "empresaId")?.Value;
            if (!long.TryParse(claimValue, out var empresaId) || empresaId <= 0)
            {
                throw new InvalidOperationException("El token autenticado no contiene un empresaId válido.");
            }

            return empresaId;
        }

        public static string GetSubjectOrThrow(ClaimsPrincipal principal)
        {
            var subject = principal?.Claims?.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new InvalidOperationException("El token autenticado no contiene un sujeto válido.");
            }

            return subject;
        }
    }
}
