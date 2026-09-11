using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    /// <summary>Servicio del front para el catálogo de Clientes (passthrough + unwrap).</summary>
    public class ClienteService
    {
        private readonly HttpClientConnection _httpClient;

        public ClienteService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>Get-by-id: hace unwrap y devuelve la entidad o null.</summary>
        public async Task<Cliente> ObtenerClientePorId(long id)
        {
            var response = await _httpClient.ObtenerClientePorId(id);
            if (response.IsSuccess && response.Response != null)
            {
                return response.Response;
            }

            return null;
        }

        public async Task<ModelResponse<Cliente>> GuardarOActualizarCliente(Cliente cliente)
        {
            return await _httpClient.GuardarOActualizarCliente(cliente);
        }

        public async Task<ModelResponse> EliminarCliente(Cliente cliente)
        {
            return await _httpClient.EliminarCliente(cliente);
        }

        public async Task<ModelResponse<List<Cliente>>> ConsultarTodosLosClientes()
        {
            return await _httpClient.ObtenerTodosLosClientes();
        }
    }
}
