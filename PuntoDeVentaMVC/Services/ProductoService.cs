using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.Services
{
    /// <summary>Servicio del front para el catálogo de Productos (passthrough + unwrap).</summary>
    public class ProductoService
    {
        private readonly HttpClientConnection _httpClient;

        public ProductoService(HttpClientConnection httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>Get-by-id: hace unwrap y devuelve el DTO o null.</summary>
        public async Task<ProductoDTO> ObtenerProductoPorId(long id)
        {
            var response = await _httpClient.ObtenerProductoPorId(id);
            if (response.IsSuccess && response.Response != null)
            {
                return response.Response;
            }

            return null;
        }

        /// <summary>Get-by-código: hace unwrap y devuelve el DTO o null.</summary>
        public async Task<ProductoDTO> ObtenerProductoPorCodigo(string codigo)
        {
            var response = await _httpClient.ObtenerProductoPorCodigo(codigo);
            if (response.IsSuccess && response.Response != null)
            {
                return response.Response;
            }

            return null;
        }

        public async Task<ModelResponse<ProductoDTO>> GuardarOActualizarProducto(Producto producto)
        {
            return await _httpClient.GuardarOActualizarProducto(producto);
        }

        public async Task<ModelResponse> EliminarProducto(Producto producto)
        {
            return await _httpClient.EliminarProducto(producto);
        }

        public async Task<ModelResponse<List<ProductoDTO>>> ConsultarTodosLosProductos()
        {
            return await _httpClient.ObtenerTodosLosProductos();
        }
    }
}
