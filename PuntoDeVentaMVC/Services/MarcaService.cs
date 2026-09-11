using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    /// <summary>Servicio del front para el catálogo de Marcas (passthrough + unwrap).</summary>
    public class MarcaService
    {
        private readonly HttpClientConnection _httpClient;

        public MarcaService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>Get-by-id: hace unwrap y devuelve la entidad o null.</summary>
        public async Task<Marca> ObtenerMarcaPorId(long id)
        {
            var response = await _httpClient.ObtenerMarcaPorId(id);
            if (response.IsSuccess && response.Response != null)
            {
                return response.Response;
            }

            return null;
        }

        public async Task<ModelResponse<Marca>> GuardarOActualizarMarca(Marca marca)
        {
            return await _httpClient.GuardarOActualizarMarca(marca);
        }

        public async Task<ModelResponse> EliminarMarca(Marca marca)
        {
            return await _httpClient.EliminarMarca(marca);
        }

        public async Task<ModelResponse<List<Marca>>> ConsultarTodasLasMarcas()
        {
            return await _httpClient.ObtenerTodasLasMarcas();
        }
    }
}
