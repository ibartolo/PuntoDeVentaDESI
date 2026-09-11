namespace PuntoDeVentaEntities.Caja
{
    /// <summary>Detalle del corte por metodo de pago, con adjuntos (folio/tickets/comprobante).</summary>
    public class CorteDetalle : BaseObject
    {
        public long CorteId { get; set; }
        public string MetodoPago { get; set; }
        public decimal Monto { get; set; }
        public string FolioCobro { get; set; }
        public string TicketCobroUrl { get; set; }
        public string TicketSistemaUrl { get; set; }
        public string ComprobanteUrl { get; set; }
    }
}
