using Newtonsoft.Json;
using PuntoDeVentaEntities;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaMVC.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PuntoDeVentaMVC.Controllers
{
    /// <summary>
    /// Controlador de catálogos POS. Dos regiones: Views (render) y Data Access (AJAX).
    /// Agrupa Sucursales, Categorías, Clientes y Proveedores.
    /// </summary>
    public class CatalogsController : BaseController
    {
        private readonly SucursalService _sucursalService;
        private readonly CategoriaService _categoriaService;
        private readonly ClienteService _clienteService;
        private readonly ProveedorService _proveedorService;
        private readonly ProductoService _productoService;
        private readonly PrecioService _precioService;
        private readonly MarcaService _marcaService;

        public CatalogsController()
        {
            _sucursalService = new SucursalService(httpClientConnection);
            _categoriaService = new CategoriaService(httpClientConnection);
            _clienteService = new ClienteService(httpClientConnection);
            _proveedorService = new ProveedorService(httpClientConnection);
            _productoService = new ProductoService(httpClientConnection);
            _precioService = new PrecioService(httpClientConnection);
            _marcaService = new MarcaService(httpClientConnection);
        }

        #region Views

        public async Task<ActionResult> Branch(long id = 0)
        {
            var sucursal = new Sucursal();

            if (id > 0)
            {
                var sucursalResponse = await _sucursalService.ObtenerSucursalPorId(id);
                if (sucursalResponse != null)
                {
                    sucursal = sucursalResponse;
                }
                else
                {
                    ViewBag.ErrorMessage = "No se encontró la sucursal.";
                }
            }

            return View("~/Views/Catalogs/Branch.cshtml", sucursal);
        }

        public async Task<ActionResult> Category(long id = 0)
        {
            var categoria = new Categoria();

            if (id > 0)
            {
                var categoriaResponse = await _categoriaService.ObtenerCategoriaPorId(id);
                if (categoriaResponse != null)
                {
                    categoria = categoriaResponse;
                }
                else
                {
                    ViewBag.ErrorMessage = "No se encontró la categoría.";
                }
            }

            var categoriasResponse = await _categoriaService.ConsultarTodasLasCategorias();
            ViewBag.CategoriasPadre = categoriasResponse != null && categoriasResponse.IsSuccess
                ? categoriasResponse.Response
                : new System.Collections.Generic.List<CategoriaDTO>();

            return View("~/Views/Catalogs/Category.cshtml", categoria);
        }

        public async Task<ActionResult> Client(long id = 0)
        {
            var cliente = new Cliente();

            if (id > 0)
            {
                var clienteResponse = await _clienteService.ObtenerClientePorId(id);
                if (clienteResponse != null)
                {
                    cliente = clienteResponse;
                }
                else
                {
                    ViewBag.ErrorMessage = "No se encontró el cliente.";
                }
            }

            return View("~/Views/Catalogs/Client.cshtml", cliente);
        }

        public async Task<ActionResult> Supplier(long id = 0)
        {
            var proveedor = new Proveedor();

            if (id > 0)
            {
                var proveedorResponse = await _proveedorService.ObtenerProveedorPorId(id);
                if (proveedorResponse != null)
                {
                    proveedor = proveedorResponse;
                }
                else
                {
                    ViewBag.ErrorMessage = "No se encontró el proveedor.";
                }
            }

            return View("~/Views/Catalogs/Supplier.cshtml", proveedor);
        }

        public async Task<ActionResult> Product(long id = 0)
        {
            var producto = new Producto();

            if (id > 0)
            {
                var productoDto = await _productoService.ObtenerProductoPorId(id);
                if (productoDto != null)
                {
                    producto = MapearProducto(productoDto);
                }
                else
                {
                    ViewBag.ErrorMessage = "No se encontró el producto.";
                }
            }

            var categoriasResponse = await _categoriaService.ConsultarTodasLasCategorias();
            ViewBag.Categorias = categoriasResponse != null && categoriasResponse.IsSuccess
                ? categoriasResponse.Response
                : new List<CategoriaDTO>();

            var marcasResponse = await _marcaService.ConsultarTodasLasMarcas();
            ViewBag.Marcas = marcasResponse != null && marcasResponse.IsSuccess
                ? marcasResponse.Response
                : new List<Marca>();

            return View("~/Views/Catalogs/Product.cshtml", producto);
        }

        #endregion

        #region Data Access - Sucursal

        public async Task<string> ConsultarTodasLasSucursales()
        {
            var response = await _sucursalService.ConsultarTodasLasSucursales();
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarSucursalPorId(long id)
        {
            var response = await _sucursalService.ObtenerSucursalPorId(id);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> GuardarActualizarSucursales(Sucursal sucursal)
        {
            AplicarAuditoria(sucursal);
            var response = await _sucursalService.GuardarOActualizarSucursal(sucursal);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> EliminarSucursales(Sucursal sucursal)
        {
            var response = await _sucursalService.EliminarSucursal(sucursal);
            return JsonConvert.SerializeObject(response);
        }

        #endregion

        #region Data Access - Categoria

        public async Task<string> ConsultarTodasLasCategorias()
        {
            var response = await _categoriaService.ConsultarTodasLasCategorias();
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarCategoriasPorPadre(long id)
        {
            var response = await _categoriaService.ConsultarCategoriasPorPadre(id);
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarCategoriaPorId(long id)
        {
            var response = await _categoriaService.ObtenerCategoriaPorId(id);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> GuardarActualizarCategorias(Categoria categoria)
        {
            AplicarAuditoria(categoria);
            var response = await _categoriaService.GuardarOActualizarCategoria(categoria);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> EliminarCategorias(Categoria categoria)
        {
            var response = await _categoriaService.EliminarCategoria(categoria);
            return JsonConvert.SerializeObject(response);
        }

        #endregion

        #region Data Access - Cliente

        public async Task<string> ConsultarTodosLosClientes()
        {
            var response = await _clienteService.ConsultarTodosLosClientes();
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarClientePorId(long id)
        {
            var response = await _clienteService.ObtenerClientePorId(id);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> GuardarActualizarClientes(Cliente cliente)
        {
            AplicarAuditoria(cliente);
            var response = await _clienteService.GuardarOActualizarCliente(cliente);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> EliminarClientes(Cliente cliente)
        {
            var response = await _clienteService.EliminarCliente(cliente);
            return JsonConvert.SerializeObject(response);
        }

        #endregion

        #region Data Access - Proveedor

        public async Task<string> ConsultarTodosLosProveedores()
        {
            var response = await _proveedorService.ConsultarTodosLosProveedores();
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarProveedorPorId(long id)
        {
            var response = await _proveedorService.ObtenerProveedorPorId(id);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> GuardarActualizarProveedores(Proveedor proveedor)
        {
            AplicarAuditoria(proveedor);
            var response = await _proveedorService.GuardarOActualizarProveedor(proveedor);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> EliminarProveedores(Proveedor proveedor)
        {
            var response = await _proveedorService.EliminarProveedor(proveedor);
            return JsonConvert.SerializeObject(response);
        }

        #endregion

        #region Data Access - Producto

        public async Task<string> ConsultarTodosLosProductos()
        {
            var response = await _productoService.ConsultarTodosLosProductos();
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarProductoPorId(long id)
        {
            var response = await _productoService.ObtenerProductoPorId(id);
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarProductoPorCodigo(string codigo)
        {
            var response = await _productoService.ObtenerProductoPorCodigo(codigo);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> GuardarActualizarProducto(Producto producto)
        {
            AplicarAuditoria(producto);
            var response = await _productoService.GuardarOActualizarProducto(producto);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> EliminarProducto(Producto producto)
        {
            var response = await _productoService.EliminarProducto(producto);
            return JsonConvert.SerializeObject(response);
        }

        #endregion

        #region Data Access - Precio

        public async Task<string> ConsultarPrecioActivo(long productoId)
        {
            var response = await _precioService.ObtenerPrecioActivo(productoId);
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarPreciosPorProducto(long productoId)
        {
            var response = await _precioService.ConsultarPreciosPorProducto(productoId);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public async Task<string> GuardarActualizarPrecio(Precio precio)
        {
            AplicarAuditoria(precio);
            var response = await _precioService.GuardarOActualizarPrecio(precio);
            return JsonConvert.SerializeObject(response);
        }

        #endregion

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

        private static Producto MapearProducto(ProductoDTO dto)
        {
            return new Producto
            {
                Id = dto.Id,
                EmpresaId = dto.EmpresaId,
                CategoriaId = dto.CategoriaId,
                MarcaId = dto.MarcaId,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                FotoUrl = dto.FotoUrl,
                Codigo = dto.Codigo,
                TipoProducto = dto.TipoProducto,
                UnidadMedida = dto.UnidadMedida,
                StockMinimo = dto.StockMinimo,
                Estatus = dto.Estatus,
                CreadoPor = dto.CreadoPor,
                FechaCreacion = dto.FechaCreacion,
                ModificadoPor = dto.ModificadoPor,
                FechaModificacion = dto.FechaModificacion
            };
        }
    }
}
