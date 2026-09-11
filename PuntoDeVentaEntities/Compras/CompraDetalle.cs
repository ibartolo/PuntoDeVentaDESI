namespace PuntoDeVentaEntities.Compras
{
    /// <summary>Detalle de una compra.</summary>
    public class CompraDetalle : BaseObject
    {
        public long CompraId { get; set; }
        public long ProductoId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal Importe { get; set; }
    }
}
