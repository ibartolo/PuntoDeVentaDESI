namespace PuntoDeVentaEntities.Ventas
{
    /// <summary>Detalle de la devolucion: por producto, con opcion de regresar a stock.</summary>
    public class DevolucionDetalle : BaseObject
    {
        public long CancelacionId { get; set; }
        public long VentaDetalleId { get; set; }
        public long ProductoId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Importe { get; set; }
        public bool RegresaAStock { get; set; }
    }
}
