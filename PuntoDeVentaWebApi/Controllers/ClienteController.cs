using System.Collections.Generic;
using System.Web.Http;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.Services;

namespace PuntoDeVentaWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Cliente")]
    public class ClienteController : BaseController
    {
        private readonly ClienteService _clienteService;

        public ClienteController()
        {
            _clienteService = new ClienteService();
        }

        [HttpGet, Route("List")]
        public ModelResponse<List<Cliente>> ObtenerTodosLosClientes()
        {
            return _clienteService.ObtenerClientes(ObtenerEmpresaIdDesdeClaim());
        }

        [HttpGet, Route("{id:long}")]
        public ModelResponse<Cliente> ObtenerClientePorId(long id)
        {
            return _clienteService.ObtenerClientePorId(ObtenerEmpresaIdDesdeClaim(), id);
        }

        [HttpPost, Route("Guardar")]
        public ModelResponse<Cliente> GuardarOActualizarCliente(Cliente cliente)
        {
            return _clienteService.GuardarOActualizarCliente(ObtenerEmpresaIdDesdeClaim(), cliente, User.Identity.Name);
        }

        [HttpDelete, Route("Eliminar")]
        public ModelResponse EliminarCliente(Cliente cliente)
        {
            return _clienteService.EliminarCliente(ObtenerEmpresaIdDesdeClaim(), cliente.Id, User.Identity.Name);
        }
    }
}
