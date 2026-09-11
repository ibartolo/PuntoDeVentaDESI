using PuntoDeVentaEntities.Seguridad;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.DAL
{
    public partial class HttpClientConnection
    {
        /// <summary>Páginas/menús visibles para el usuario de la sesión (filtradas por rol).</summary>
        public async Task<ModelResponse<List<Pagina>>> ObtenerPaginasPorUsuario()
        {
            var usuarioId = token?.UserID ?? 0;
            return await RequestAsync<List<Pagina>>($"api/Pagina/Usuario/{usuarioId}", HttpMethod.Get, null, token?.Token?.access_token);
        }
    }
}
