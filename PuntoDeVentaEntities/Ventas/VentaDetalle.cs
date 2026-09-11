namespace PuntoDeVentaEntities.Ventas
{
    /// <summary>Detalle de una venta.</summary>
    public class VentaDetalle : BaseObject
    {
        public long VentaId { get; set; }
        public long ProductoId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Importe { get; set; }
    }
}
