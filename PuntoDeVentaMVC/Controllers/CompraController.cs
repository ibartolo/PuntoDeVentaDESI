using Newtonsoft.Json;
using PuntoDeVentaEntities;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Compras;
using PuntoDeVentaMVC.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PuntoDeVentaMVC.Controllers
{
    /// <summary>
    /// Controlador del historial de compras a proveedor. Dos regiones: Views (render) y Data Access (AJAX).
    /// </summary>
    public class CompraController : BaseController
    {
        private readonly CompraService _compraService;
        private readonly SucursalService _sucursalService;
        private readonly ProveedorService _proveedorService;
        private readonly ProductoService _productoService;

        public CompraController()
        {
            _compraService = new CompraService(httpClientConnection);
            _sucursalService = new SucursalService(httpClientConnection);
            _proveedorService = new ProveedorService(httpClientConnection);
            _productoService = new ProductoService(httpClientConnection);
        }

        #region Views

        public async Task<ActionResult> Index(long id = 0)
        {
            var compra = new CompraDTO();

            if (id > 0)
            {
                var compraResponse = await _compraService.ObtenerCompraPorId(id);
                if (compraResponse != null)
                {
                    compra = compraResponse;
                }
                else
                {
                    ViewBag.ErrorMessage = "No se encontró la compra.";
                }
            }

            var sucursalesResponse = await _sucursalService.ConsultarTodasLasSucursales();
            ViewBag.Sucursales = sucursalesResponse != null && sucursalesResponse.IsSuccess
                ? sucursalesResponse.Response
                : new List<Sucursal>();

            var proveedoresResponse = await _proveedorService.ConsultarTodosLosProveedores();
            ViewBag.Proveedores = proveedoresResponse != null && proveedoresResponse.IsSuccess
                ? proveedoresResponse.Response
                : new List<Proveedor>();

            ViewBag.DetalleJson = JsonConvert.SerializeObject(compra.Detalle ?? new List<CompraDetalleDTO>());

            return View("~/Views/Compras/Index.cshtml", compra);
        }

        #endregion

        #region Data Access

        public async Task<string> ConsultarTodasLasCompras(long sucursalId = 0)
        {
            var response = await _compraService.ConsultarTodasLasCompras(sucursalId);
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarCompraPorId(long id)
        {
            var response = await _compraService.ObtenerCompraPorId(id);
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarProductos()
        {
            var response = await _productoService.ConsultarTodosLosProductos();
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> GuardarCompra(CompraDTO compra)
        {
            AplicarAuditoria(compra);
            var response = await _compraService.GuardarCompra(compra);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> EliminarCompra(CompraDTO compra)
        {
            var response = await _compraService.EliminarCompra(compra);
            return JsonConvert.SerializeObject(response);
        }

        private void AplicarAuditoria(BaseObject o)
        {
            var tc = SessionHelperUser();
            if (o.Id == 0)
            {
                o.CreadoPor = tc ?? "system";
                o.FechaCreacion = System.DateTime.Now;
            }
            else
            {
                o.ModificadoPor = tc ?? "system";
                o.FechaModificacion = System.DateTime.Now;
            }

            o.Estatus = true;
        }

        private string SessionHelperUser()
        {
            return PuntoDeVentaMVC.Helpers.SessionHelper.GetSessionUser()?.UserName;
        }

        #endregion
    }
}
