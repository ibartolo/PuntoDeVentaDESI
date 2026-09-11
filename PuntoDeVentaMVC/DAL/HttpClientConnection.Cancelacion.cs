using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaEntities.Ventas;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.DAL
{
    public partial class HttpClientConnection
    {
        public async Task<ModelResponse<List<CancelacionDTO>>> ObtenerTodasLasCancelaciones(long sucursalId = 0)
        {
            return await RequestAsync<List<CancelacionDTO>>($"api/Cancelacion/List?sucursalId={sucursalId}",
                HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<CancelacionDTO>> ObtenerCancelacionPorId(long id)
        {
            return await RequestAsync<CancelacionDTO>($"api/Cancelacion/{id}", HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<CancelacionDTO>> GuardarCancelacion(CancelacionDTO cancelacion)
        {
            MappingColumSecurity(cancelacion);
            return await RequestAsync<CancelacionDTO>("api/Cancelacion/Guardar", HttpMethod.Post, cancelacion, token?.Token?.access_token);
        }
    }
}
