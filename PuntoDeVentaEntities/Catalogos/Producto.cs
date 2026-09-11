namespace PuntoDeVentaEntities.Catalogos
{
    /// <summary>
    /// Producto del catalogo. Codigo unico (barras o QR, uno solo). Marca real o generica.
    /// Tipo: Comprado (proveedor) o Fabricado (sin proveedor).
    /// </summary>
    public class Producto : BaseObject
    {
        public long EmpresaId { get; set; }
        public long? CategoriaId { get; set; }
        public long? MarcaId { get; set; }
        public string MarcaTexto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string FotoUrl { get; set; }
        public string Codigo { get; set; }
        public string TipoProducto { get; set; }
        public string UnidadMedida { get; set; }
        public decimal StockMinimo { get; set; }
    }

    /// <summary>DTO de producto con datos resueltos de categoria/marca/precio/stock.</summary>
    public class ProductoDTO : BaseObject
    {
        public long EmpresaId { get; set; }
        public long? CategoriaId { get; set; }
        public string CategoriaNombre { get; set; }
        public long? MarcaId { get; set; }
        public string MarcaNombre { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string FotoUrl { get; set; }
        public string Codigo { get; set; }
        public string TipoProducto { get; set; }
        public string UnidadMedida { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal StockDisponible { get; set; }
    }
}
