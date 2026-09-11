using Newtonsoft.Json;
using PuntoDeVentaEntities.Autenticacion;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.Helpers;
using PuntoDeVentaMVC.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PuntoDeVentaMVC.Controllers
{
    /// <summary>
    /// Controlador de inicio, autenticación (login en 2 pasos) y páginas base.
    /// El acceso está protegido por el filtro global AuthenticationFilter (allow-list).
    /// </summary>
    public class HomeController : BaseController
    {
        private readonly AutenticacionService _autenticacionService;
        private readonly EmpresaService _empresaService;

        public HomeController()
        {
            _autenticacionService = new AutenticacionService(httpClientConnection);
            _empresaService = new EmpresaService(httpClientConnection);
        }

        #region Views

        public ActionResult Autentication()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Configuration()
        {
            return View();
        }

        public ActionResult RecoverPassword(string id)
        {
            ViewBag.Token = id;
            return View();
        }

        public ActionResult NewCompany()
        {
            return View();
        }

        public ActionResult AccesoDenegado()
        {
            return View();
        }

        public async Task<ActionResult> MenusUser()
        {
            var response = await httpClientConnection.ObtenerPaginasPorUsuario();
            var paginas = response != null && response.IsSuccess && response.Response != null
                ? response.Response
                : new List<Pagina>();

            return PartialView(paginas);
        }

        #endregion

        #region Data Access

        /// <summary>
        /// Login en 2 pasos: (1) autenticar credenciales (mensaje específico, sin token) y
        /// (2) obtener el token OAuth. Devuelve un JSON string (doble encode).
        /// </summary>
        [HttpPost]
        public async Task<string> LogIn(string user, string pass)
        {
            var mr = new ModelResponse();

            Log.Information("LogIn iniciado para usuario {Usuario}", user);

            try
            {
                var response = await _autenticacionService.AutenticarUsuario(new Usuario
                {
                    NombreUsuario = user,
                    Contrasena = pass
                });

                Log.Information("LogIn paso 1 (autenticar) para {Usuario}: IsSuccess={IsSuccess}, Message={Message}", user, response?.IsSuccess, response?.Message);

                if (response == null || !response.IsSuccess || response.Response == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = response?.Message ?? "Usuario o contraseña incorrectos";
                    return JsonConvert.SerializeObject(mr);
                }

                var token = await httpClientConnection.GetToken(user, pass);
                if (token == null)
                {
                    Log.Warning("LogIn paso 2 (token) FALLO para {Usuario}: token nulo", user);
                    mr.IsSuccess = false;
                    mr.Message = "Error de usuario o contraseña";
                    return JsonConvert.SerializeObject(mr);
                }

                var usuario = response.Response;
                token.ExpirationDate = DateTime.Now.AddSeconds(token.expires_in);

                var tokenCookie = new TokenCookie
                {
                    Token = token,
                    UserID = usuario.Id,
                    EmpresaID = usuario.EmpresaId,
                    UserName = user,
                    ProfileImage = usuario.ImagenPerfil,
                    UserAvatar = GenerarAvatarIniciales(usuario.NombreUsuario)
                };

                SessionHelper.CreateSession(JsonConvert.SerializeObject(tokenCookie));

                mr.IsSuccess = true;
                mr.Message = "Ok";
                Log.Information("LogIn exitoso para {Usuario}", user);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "LogIn error para {Usuario}", user);
                mr.IsSuccess = false;
                mr.Message = ex.Message;
            }

            return JsonConvert.SerializeObject(mr);
        }

        public ActionResult LogOut()
        {
            SessionHelper.CloseSession();
            return RedirectToAction("Autentication");
        }

        public async Task<string> ValidarToken(string id)
        {
            var response = await _autenticacionService.ValidarTokenRecuperacion(id);
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> RestablecerContrasenia(string token, string nuevaContrasena)
        {
            var response = await _autenticacionService.RestablecerContrasenia(token, nuevaContrasena);
            return JsonConvert.SerializeObject(response);
        }

        /// <summary>Solicita el correo de recuperación a partir del usuario/correo indicado (SYNC-4).</summary>
        [HttpPost]
        public async Task<string> ValidarRecetearContrasenia(Usuario usuario)
        {
            var valor = usuario?.Correo;
            if (string.IsNullOrWhiteSpace(valor))
            {
                valor = usuario?.NombreUsuario;
            }

            var response = await _autenticacionService.SolicitarRecuperacion(valor);
            return JsonConvert.SerializeObject(response);
        }

        /// <summary>Registra una nueva empresa desde el formulario público de alta (SYNC-5).</summary>
        [HttpPost]
        public async Task<string> GuardarNuevaEmpresa(Empresa empresa)
        {
            var response = await _empresaService.GuardarEmpresa(empresa);
            return JsonConvert.SerializeObject(response);
        }

        /// <summary>Persiste el tema claro/oscuro del usuario en su cookie (SYNC-3).</summary>
        [HttpPost]
        public string GuardarTema(string tema)
        {
            var mr = new ModelResponse();
            try
            {
                if (tema != "light" && tema != "dark")
                {
                    tema = "light";
                }

                var cookieName = ThemeHelper.GetCookieName(tokenCookie);
                var cookie = new HttpCookie(cookieName, tema)
                {
                    Expires = DateTime.Now.AddYears(1)
                };
                Response.Cookies.Add(cookie);

                mr.IsSuccess = true;
                mr.Message = "Tema guardado correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GuardarTema error");
                mr.IsSuccess = false;
                mr.Message = "No se pudo guardar la preferencia de tema.";
            }

            return JsonConvert.SerializeObject(mr);
        }

        #endregion
    }
}
