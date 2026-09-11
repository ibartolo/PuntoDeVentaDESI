namespace PuntoDeVentaEntities.Inventario
{
    /// <summary>Movimiento de inventario (ingreso, ajuste +/-, devolucion, venta).</summary>
    public class StockMovimiento : BaseObject
    {
        public long EmpresaId { get; set; }
        public long SucursalId { get; set; }
        public long ProductoId { get; set; }
        public string TipoMovimiento { get; set; }
        public decimal Cantidad { get; set; }
        public decimal ExistenciaAnterior { get; set; }
        public decimal ExistenciaNueva { get; set; }
        public string Motivo { get; set; }
        public long? ReferenciaId { get; set; }
    }
}
