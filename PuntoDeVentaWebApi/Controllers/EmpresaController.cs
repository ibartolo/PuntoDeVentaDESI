using System.Web.Http;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [RoutePrefix("api/Empresa")]
    public class EmpresaController : BaseController
    {
        private readonly EmpresaService _empresaService;

        public EmpresaController()
        {
            _empresaService = new EmpresaService();
        }

        /// <summary>Alta pública de una empresa nueva (sin token).</summary>
        [AllowAnonymous]
        [HttpPost, Route("registrar")]
        public ModelResponse<Empresa> Registrar(Empresa empresa)
        {
            return _empresaService.GuardarEmpresa(empresa);
        }
    }
}
