using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Autenticacion;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [RoutePrefix("api/Autenticacion")]
    public class AutenticacionController : BaseController
    {
        private readonly AutenticacionService _autenticacionService;

        public AutenticacionController()
        {
            _autenticacionService = new AutenticacionService();
        }

        [AllowAnonymous]
        [HttpPost, Route("autenticar")]
        public ModelResponse<Usuario> Autenticar(Usuario usuario)
        {
            return _autenticacionService.AutenticarUsuario(usuario);
        }

        #region Usuarios

        [Authorize]
        [HttpGet, Route("User/List")]
        public ModelResponse<List<UsuarioDTO>> ObtenerUsuarios()
        {
            return _autenticacionService.ObtenerUsuarios(ObtenerEmpresaIdDesdeClaim());
        }

        [Authorize]
        [HttpGet, Route("User/{id:long}")]
        public ModelResponse<Usuario> ObtenerUsuarioPorId(long id)
        {
            return _autenticacionService.ObtenerUsuarioPorId(ObtenerEmpresaIdDesdeClaim(), id);
        }

        [Authorize]
        [HttpPost, Route("User")]
        public ModelResponse<Usuario> GuardarOActualizarUsuario(Usuario usuario)
        {
            return _autenticacionService.GuardarOActualizarUsuario(ObtenerEmpresaIdDesdeClaim(), usuario, User.Identity.Name);
        }

        [Authorize]
        [HttpDelete, Route("User")]
        public ModelResponse EliminarUsuario(Usuario usuario)
        {
            return _autenticacionService.EliminarUsuario(ObtenerEmpresaIdDesdeClaim(), usuario.Id, User.Identity.Name);
        }

        #endregion

        #region Recuperación de contraseña

        [AllowAnonymous]
        [HttpPost, Route("solicitarRecuperacion")]
        public ModelResponse SolicitarRecuperacion(Usuario usuario)
        {
            var valor = usuario?.NombreUsuario;
            if (string.IsNullOrWhiteSpace(valor))
            {
                valor = usuario?.Correo;
            }

            return _autenticacionService.SolicitarRecuperacion(valor);
        }

        [AllowAnonymous]
        [HttpGet, Route("validarToken/{token}")]
        public ModelResponse<TokenRecuperacionDTO> ValidarToken(string token)
        {
            return _autenticacionService.ValidarTokenRecuperacion(token);
        }

        [AllowAnonymous]
        [HttpPost, Route("restablecerContrasenia")]
        public ModelResponse RestablecerContrasenia(RestablecerContraseniaRequest request)
        {
            return _autenticacionService.RestablecerContrasenia(request?.Token, request?.NuevaContrasena);
        }

        #endregion
    }
}
