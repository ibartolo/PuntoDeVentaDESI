using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    /// <summary>Servicio del front para el catálogo de Sucursales (passthrough + unwrap).</summary>
    public class SucursalService
    {
        private readonly HttpClientConnection _httpClient;

        public SucursalService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>Get-by-id: hace unwrap y devuelve la entidad o null.</summary>
        public async Task<Sucursal> ObtenerSucursalPorId(long id)
        {
            var response = await _httpClient.ObtenerSucursalPorId(id);
            if (response.IsSuccess && response.Response != null)
            {
                return response.Response;
            }

            return null;
        }

        public async Task<ModelResponse<Sucursal>> GuardarOActualizarSucursal(Sucursal sucursal)
        {
            return await _httpClient.GuardarOActualizarSucursal(sucursal);
        }

        public async Task<ModelResponse> EliminarSucursal(Sucursal sucursal)
        {
            return await _httpClient.EliminarSucursal(sucursal);
        }

        public async Task<ModelResponse<List<Sucursal>>> ConsultarTodasLasSucursales()
        {
            return await _httpClient.ObtenerTodasLasSucursales();
        }
    }
}
