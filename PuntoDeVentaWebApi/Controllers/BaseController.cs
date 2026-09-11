using System.Security.Claims;
using System.Web.Http;
using PuntoDeVentaWebApi.DAL;

namespace PuntoDeVentaWebApi.Controllers
{
    public abstract class BaseController : ApiController
    {
        public DbWrapper dbWrapper;

        protected BaseController()
        {
            dbWrapper = new DbWrapper();
        }

        /// <summary>Lee el claim "empresaId" del token; devuelve 0 si no existe.</summary>
        public long ObtenerEmpresaIdDesdeClaim()
        {
            var identity = User?.Identity as ClaimsIdentity;
            var claim = identity?.FindFirst("empresaId");
            long empresaId;
            if (claim != null && long.TryParse(claim.Value, out empresaId))
            {
                return empresaId;
            }

            return 0;
        }

        /// <summary>Lee el claim "usuarioId" del token; devuelve 0 si no existe.</summary>
        public long ObtenerUsuarioIdDesdeClaim()
        {
            var identity = User?.Identity as ClaimsIdentity;
            var claim = identity?.FindFirst("usuarioId");
            long usuarioId;
            if (claim != null && long.TryParse(claim.Value, out usuarioId))
            {
                return usuarioId;
            }

            return 0;
        }
    }
}
