using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.DAL;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    /// <summary>Servicio del front para el alta de Empresa (registro público).</summary>
    public class EmpresaService
    {
        private readonly HttpClientConnection _httpClient;

        public EmpresaService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ModelResponse<Empresa>> GuardarEmpresa(Empresa empresa)
        {
            return await _httpClient.GuardarEmpresa(empresa);
        }
    }
}
