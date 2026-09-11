using PuntoDeVentaEntities.Seguridad;
using System.Web;

namespace PuntoDeVentaMVC.Helpers
{
    /// <summary>
    /// Preferencia de tema (claro/oscuro) almacenada en una cookie propia del usuario.
    /// No es la cookie de sesión; expira en 1 año y se renueva cada vez que el usuario cambia el tema.
    /// </summary>
    public static class ThemeHelper
    {
        public const string CookiePrefix = "TemaUsuario_";

        /// <summary>Devuelve el nombre de la cookie de tema para el usuario indicado.</summary>
        public static string GetCookieName(TokenCookie tokenCookie)
        {
            if (tokenCookie != null && tokenCookie.UserID > 0)
            {
                return $"{CookiePrefix}{tokenCookie.UserID}";
            }

            return CookiePrefix;
        }

        /// <summary>Lee el tema guardado en la cookie ('light' o 'dark'). Por defecto 'light'.</summary>
        public static string GetTema(HttpRequestBase request, TokenCookie tokenCookie)
        {
            var cookieName = GetCookieName(tokenCookie);
            var cookie = request?.Cookies[cookieName];
            if (cookie != null && (cookie.Value == "light" || cookie.Value == "dark"))
            {
                return cookie.Value;
            }

            return "light";
        }

        /// <summary>Clase CSS a aplicar en el body según el tema ('dark-theme' o cadena vacía).</summary>
        public static string GetTemaClase(HttpRequestBase request, TokenCookie tokenCookie)
        {
            return GetTema(request, tokenCookie) == "dark" ? "dark-theme" : "";
        }
    }
}
