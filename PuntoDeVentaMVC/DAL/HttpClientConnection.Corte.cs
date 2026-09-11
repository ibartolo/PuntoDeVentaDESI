using PuntoDeVentaEntities.Caja;
using PuntoDeVentaEntities.Seguridad;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.DAL
{
    public partial class HttpClientConnection
    {
        public async Task<ModelResponse<List<CorteDTO>>> ObtenerTodosLosCortes(long sucursalId = 0)
        {
            return await RequestAsync<List<CorteDTO>>($"api/Corte/List?sucursalId={sucursalId}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<CorteDTO>> ObtenerCortePorId(long id)
        {
            return await RequestAsync<CorteDTO>($"api/Corte/{id}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<CorteDTO>> GuardarCorte(CorteDTO corte)
        {
            MappingColumSecurity(corte);
            return await RequestAsync<CorteDTO>("api/Corte/Guardar", HttpMethod.Post, corte, token?.Token?.access_token);
        }
    }
}
