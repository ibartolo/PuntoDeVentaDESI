using Newtonsoft.Json;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Inventario;
using PuntoDeVentaMVC.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PuntoDeVentaMVC.Controllers
{
    /// <summary>
    /// Controlador de inventario (stock por sucursal y movimientos de ingreso/ajuste/devolución).
    /// Dos regiones: Views (render) y Data Access (AJAX).
    /// </summary>
    public class StockController : BaseController
    {
        private readonly StockService _stockService;
        private readonly SucursalService _sucursalService;

        public StockController()
        {
            _stockService = new StockService(httpClientConnection);
            _sucursalService = new SucursalService(httpClientConnection);
        }

        #region Views

        public async Task<ActionResult> Index()
        {
            var sucursalesResponse = await _sucursalService.ConsultarTodasLasSucursales();
            var sucursales = sucursalesResponse != null && sucursalesResponse.IsSuccess && sucursalesResponse.Response != null
                ? sucursalesResponse.Response
                : new List<Sucursal>();

            ViewBag.Sucursales = MappingPropertiToDropDownList(sucursales, "Id", "Nombre");

            return View("~/Views/Stock/Index.cshtml", new StockMovimientoDTO());
        }

        #endregion

        #region Data Access

        public async Task<string> ConsultarStockPorSucursal(long sucursalId)
        {
            var response = await _stockService.ConsultarStockPorSucursal(sucursalId);
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarStock(long sucursalId, long productoId)
        {
            var response = await _stockService.ConsultarStock(sucursalId, productoId);
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarMovimientos(long sucursalId = 0, long productoId = 0)
        {
            var response = await _stockService.ConsultarMovimientos(sucursalId, productoId);
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarProductos()
        {
            var response = await _stockService.ConsultarProductosParaStock();
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> RegistrarMovimientoStock(StockMovimientoDTO movimiento)
        {
            var tc = SessionHelperUser();
            movimiento.CreadoPor = tc ?? "system";
            movimiento.FechaCreacion = System.DateTime.Now;
            movimiento.Estatus = true;

            var response = await _stockService.RegistrarMovimientoStock(movimiento);
            return JsonConvert.SerializeObject(response);
        }

        private string SessionHelperUser()
        {
            return PuntoDeVentaMVC.Helpers.SessionHelper.GetSessionUser()?.UserName;
        }

        #endregion
    }
}
