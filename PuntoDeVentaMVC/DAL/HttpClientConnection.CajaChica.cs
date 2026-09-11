using Newtonsoft.Json;
using PuntoDeVentaEntities.Caja;
using PuntoDeVentaEntities.Seguridad;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.DAL
{
    public partial class HttpClientConnection
    {
        /// <summary>Caja chica abierta del usuario en la sucursal (puede venir null).</summary>
        public async Task<ModelResponse<CajaChicaDTO>> ObtenerCajaChicaAbierta(long sucursalId, long usuarioId)
        {
            var endpoint = $"api/CajaChica/Abierta?sucursalId={sucursalId}&usuarioId={usuarioId}";
            return await RequestAsync<CajaChicaDTO>(endpoint, HttpMethod.Get, null, token?.Token?.access_token);
        }

        public async Task<ModelResponse<CajaChicaDTO>> AbrirCajaChica(CajaChicaDTO caja)
        {
            MappingColumSecurity(caja);
            return await RequestAsync<CajaChicaDTO>("api/CajaChica/Abrir", HttpMethod.Post, caja, token?.Token?.access_token);
        }

        public async Task<ModelResponse> CerrarCajaChica(CajaChicaDTO caja)
        {
            MappingColumSecurity(caja);
            var result = await RequestAsync<object>("api/CajaChica/Cerrar", HttpMethod.Post, caja,
                new Func<string, string>(responseString => responseString), token?.Token?.access_token);

            return JsonConvert.DeserializeObject<ModelResponse>(result.ToString());
        }

        public async Task<ModelResponse<List<SalidaCaja>>> ObtenerSalidasCaja(long cajaChicaId)
        {
            return await RequestAsync<List<SalidaCaja>>($"api/CajaChica/Salidas/{cajaChicaId}", HttpMethod.Get, null,
                token?.Token?.access_token);
        }

        public async Task<ModelResponse<SalidaCaja>> RegistrarSalidaCaja(SalidaCaja salida)
        {
            MappingColumSecurity(salida);
            return await RequestAsync<SalidaCaja>("api/CajaChica/Salida", HttpMethod.Post, salida, token?.Token?.access_token);
        }
    }
}
