using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using PuntoDeVenta.MVC.Models;
using PuntoDeVenta.MVC.Services;

namespace PuntoDeVenta.MVC.Controllers
{
    public class AccountController : Controller
    {
        private const string ServerBearerSessionKey = "ServerBearerToken";
        private readonly AuthApiClient _authApiClient;

        public AccountController()
        {
            _authApiClient = new AuthApiClient();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var token = await _authApiClient.RequestTokenAsync(model.UserName, model.Password);
            if (token == null || string.IsNullOrWhiteSpace(token.AccessToken))
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos.");
                return View(model);
            }

            var contextResult = await _authApiClient.GetAuthenticatedContextAsync(token.AccessToken);
            if (contextResult == null || !contextResult.Success || contextResult.Data == null)
            {
                ModelState.AddModelError(string.Empty, "No se pudo validar el contexto empresarial autenticado.");
                return View(model);
            }

            var expiresUtc = DateTime.UtcNow.AddSeconds(token.ExpiresIn > 0 ? token.ExpiresIn : 21600);
            var serverToken = new ServerSessionToken
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType,
                ExpirationUtc = expiresUtc,
                Subject = contextResult.Data.Sub,
                EmpresaId = contextResult.Data.EmpresaId
            };

            Session[ServerBearerSessionKey] = serverToken;

            var authPayload = JsonConvert.SerializeObject(new
            {
                sub = serverToken.Subject,
                empresaId = serverToken.EmpresaId
            });

            var authTicket = new FormsAuthenticationTicket(
                version: 1,
                name: serverToken.Subject,
                issueDate: DateTime.Now,
                expiration: DateTime.Now.AddSeconds(token.ExpiresIn > 0 ? token.ExpiresIn : 21600),
                isPersistent: false,
                userData: authPayload,
                cookiePath: FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = Request.IsSecureConnection,
                SameSite = SameSiteMode.Lax,
                Expires = authTicket.Expiration
            };

            Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
            Response.Cookies.Add(authCookie);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session.Remove(ServerBearerSessionKey);
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}
