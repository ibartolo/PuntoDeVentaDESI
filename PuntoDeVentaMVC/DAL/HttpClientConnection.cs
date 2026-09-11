using PuntoDeVentaEntities;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.Helpers;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.DAL
{
    /// <summary>
    /// Conexión HTTP del front con la WebApi. Lee el token OAuth de la cookie de sesión.
    /// Los métodos por feature viven en partials (HttpClientConnection.&lt;Feature&gt;.cs).
    /// </summary>
    public partial class HttpClientConnection : HttpClientBase
    {
        private readonly TokenCookie token;

        public HttpClientConnection(string baseUrl = "") : base(baseUrl)
        {
            token = SessionHelper.GetSessionUser();
        }

        /// <summary>Aplica auditoría del lado cliente según sea alta (Id 0/-1) o modificación.</summary>
        public BaseObject MappingColumSecurity(BaseObject o)
        {
            var sessionUser = SessionHelper.GetSessionUser();

            if (o.Id == 0 || o.Id == -1)
            {
                o.CreadoPor = sessionUser?.UserName;
                o.FechaCreacion = SessionHelper.GetDateCenterMexico();
            }
            else
            {
                o.ModificadoPor = sessionUser?.UserName;
                o.FechaModificacion = SessionHelper.GetDateCenterMexico();
            }

            return o;
        }

        /// <summary>Transporte puro multipart (el servicio construye el contenido).</summary>
        public async Task<ModelResponse<T>> PostMultipartAsync<T>(string endpoint, MultipartFormDataContent content)
        {
            return await SendMultipartAsync<T>(endpoint, content, token?.Token?.access_token);
        }

        /// <summary>Obtiene el token OAuth (password grant) contra el endpoint /token.</summary>
        public async Task<Token> GetToken(string user, string pass)
        {
            return await TokenAsync<Token>("token",
                new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("UserName", user),
                    new KeyValuePair<string, string>("Password", pass),
                    new KeyValuePair<string, string>("client_id", ConfigurationManager.AppSettings["client_id"]),
                    new KeyValuePair<string, string>("client_secret", ConfigurationManager.AppSettings["client_secret"])
                }, "application/x-www-form-urlencoded");
        }
    }
}
