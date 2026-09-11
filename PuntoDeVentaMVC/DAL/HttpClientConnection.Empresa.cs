using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.DAL
{
    public partial class HttpClientConnection
    {
        /// <summary>Registra una nueva empresa (alta pública, sin token).</summary>
        public async Task<ModelResponse<Empresa>> GuardarEmpresa(Empresa empresa)
        {
            return await RequestAsync<Empresa>("api/Empresa/registrar", HttpMethod.Post, empresa);
        }
    }
}
