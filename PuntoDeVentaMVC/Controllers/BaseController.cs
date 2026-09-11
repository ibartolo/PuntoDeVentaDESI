using PuntoDeVentaEntities.Autenticacion;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.DAL;
using PuntoDeVentaMVC.Helpers;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

namespace PuntoDeVentaMVC.Controllers
{
    /// <summary>
    /// Controlador base del front. Expone la conexión HTTP con la WebApi, la cookie de sesión
    /// y utilidades compartidas (dropdowns y avatar).
    /// </summary>
    public class BaseController : Controller
    {
        public HttpClientConnection httpClientConnection;
        public Usuario usuarioAutenticado;
        public TokenCookie tokenCookie;
        public ModelResponse mr { get; set; }

        public BaseController()
        {
            httpClientConnection = new HttpClientConnection();
            mr = new ModelResponse();
            tokenCookie = SessionHelper.GetSessionUser();
        }

        /// <summary>Convierte una colección en SelectListItem usando reflexión (value/title/prefix).</summary>
        public List<SelectListItem> MappingPropertiToDropDownList<T>(IEnumerable<T> items, string value, string title, string prefix = "")
        {
            var list = new List<SelectListItem>();

            foreach (var r in items)
            {
                var id = r.GetType().GetProperty(value);
                var nombre = r.GetType().GetProperty(title);

                PropertyInfo segundoNombre = null;
                if (!string.IsNullOrEmpty(prefix))
                {
                    segundoNombre = r.GetType().GetProperty(prefix);
                }

                list.Add(new SelectListItem
                {
                    Value = id.GetValue(r).ToString(),
                    Text = (string.IsNullOrEmpty(prefix) && segundoNombre == null)
                        ? nombre.GetValue(r).ToString()
                        : $"{segundoNombre.GetValue(r)}-{nombre.GetValue(r)}"
                });
            }

            return list;
        }

        /// <summary>Genera las iniciales (hasta 2) del nombre de usuario para el avatar.</summary>
        public string GenerarAvatarIniciales(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                return "??";
            }

            var regex = new System.Text.RegularExpressions.Regex(@"^([a-zA-Z])[a-zA-Z]*\.?([a-zA-Z])");
            var match = regex.Match(nombreUsuario);

            if (match.Success && match.Groups.Count > 2)
            {
                var primera = match.Groups[1].Value.ToUpper();
                var segunda = match.Groups[2].Value.ToUpper();
                return $"{primera}{segunda}";
            }

            if (nombreUsuario.Length >= 2)
            {
                return $"{nombreUsuario[0].ToString().ToUpper()}{nombreUsuario[1].ToString().ToUpper()}";
            }

            return nombreUsuario.Length == 1 ? nombreUsuario.ToUpper() : "??";
        }
    }
}
