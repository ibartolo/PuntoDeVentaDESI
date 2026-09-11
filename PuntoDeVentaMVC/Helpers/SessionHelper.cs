using Newtonsoft.Json;
using PuntoDeVentaEntities.Seguridad;
using System;
using System.Web;
using System.Web.Security;

namespace PuntoDeVentaMVC.Helpers
{
    /// <summary>
    /// Gestiona la "sesión" del front, que es una cookie FormsAuthentication cuyo
    /// UserData contiene el JSON del <see cref="TokenCookie"/> (token OAuth + datos del usuario).
    /// </summary>
    public static class SessionHelper
    {
        /// <summary>Indica si existe una sesión válida (token presente y no expirado).</summary>
        public static bool ExisteSession()
        {
            var token = GetSessionUser();
            if (token == null || token.Token == null)
            {
                return false;
            }

            return token.Token.ExpirationDate >= DateTime.Now;
        }

        /// <summary>Cierra la sesión eliminando la cookie de autenticación.</summary>
        public static void CloseSession()
        {
            FormsAuthentication.SignOut();
        }

        /// <summary>Deserializa el TokenCookie almacenado en la cookie de autenticación.</summary>
        public static TokenCookie GetSessionUser()
        {
            if (HttpContext.Current != null &&
                HttpContext.Current.User != null &&
                HttpContext.Current.User.Identity is FormsIdentity identity &&
                identity.Ticket != null)
            {
                return JsonConvert.DeserializeObject<TokenCookie>(identity.Ticket.UserData);
            }

            return null;
        }

        /// <summary>
        /// Crea la cookie de sesión a partir del JSON del TokenCookie. La expiración de la
        /// cookie coincide con la expiración del token OAuth.
        /// </summary>
        public static void CreateSession(string id)
        {
            var tokenCookie = JsonConvert.DeserializeObject<TokenCookie>(id);

            const bool persist = true;
            var cookie = FormsAuthentication.GetAuthCookie("token", persist);
            cookie.Name = FormsAuthentication.FormsCookieName;
            cookie.Expires = tokenCookie.Token.ExpirationDate;

            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            var newTicket = new FormsAuthenticationTicket(
                ticket.Version,
                ticket.Name,
                ticket.IssueDate,
                cookie.Expires,
                ticket.IsPersistent,
                id);

            cookie.Value = FormsAuthentication.Encrypt(newTicket);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>Fecha/hora actual en la zona Centro de México (usada para auditoría).</summary>
        public static DateTime GetDateCenterMexico()
        {
            var timeUtc = DateTime.UtcNow;
            var cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
        }
    }
}
