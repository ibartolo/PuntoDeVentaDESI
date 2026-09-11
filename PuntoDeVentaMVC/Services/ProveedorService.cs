using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    /// <summary>Servicio del front para el catálogo de Proveedores (passthrough + unwrap).</summary>
    public class ProveedorService
    {
        private readonly HttpClientConnection _httpClient;

        public ProveedorService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>Get-by-id: hace unwrap y devuelve la entidad o null.</summary>
        public async Task<Proveedor> ObtenerProveedorPorId(long id)
        {
            var response = await _httpClient.ObtenerProveedorPorId(id);
            if (response.IsSuccess && response.Response != null)
            {
                return response.Response;
            }

            return null;
        }

        public async Task<ModelResponse<Proveedor>> GuardarOActualizarProveedor(Proveedor proveedor)
        {
            return await _httpClient.GuardarOActualizarProveedor(proveedor);
        }

        public async Task<ModelResponse> EliminarProveedor(Proveedor proveedor)
        {
            return await _httpClient.EliminarProveedor(proveedor);
        }

        public async Task<ModelResponse<List<Proveedor>>> ConsultarTodosLosProveedores()
        {
            return await _httpClient.ObtenerTodosLosProveedores();
        }
    }
}
