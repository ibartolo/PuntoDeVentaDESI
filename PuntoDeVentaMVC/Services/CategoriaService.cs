using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    /// <summary>Servicio del front para el catálogo de Categorías (passthrough + unwrap).</summary>
    public class CategoriaService
    {
        private readonly HttpClientConnection _httpClient;

        public CategoriaService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>Get-by-id: hace unwrap y devuelve la entidad o null.</summary>
        public async Task<Categoria> ObtenerCategoriaPorId(long id)
        {
            var response = await _httpClient.ObtenerCategoriaPorId(id);
            if (response.IsSuccess && response.Response != null)
            {
                return response.Response;
            }

            return null;
        }

        public async Task<ModelResponse<Categoria>> GuardarOActualizarCategoria(Categoria categoria)
        {
            return await _httpClient.GuardarOActualizarCategoria(categoria);
        }

        public async Task<ModelResponse> EliminarCategoria(Categoria categoria)
        {
            return await _httpClient.EliminarCategoria(categoria);
        }

        public async Task<ModelResponse<List<CategoriaDTO>>> ConsultarTodasLasCategorias()
        {
            return await _httpClient.ObtenerTodasLasCategorias();
        }

        public async Task<ModelResponse<List<CategoriaDTO>>> ConsultarCategoriasPorPadre(long id)
        {
            return await _httpClient.ObtenerCategoriasPorPadre(id);
        }
    }
}
