using System;

namespace PuntoDeVentaEntities.Inventario
{
    /// <summary>Existencia de un producto en una sucursal (stock independiente por sucursal).</summary>
    public class Stock : BaseObject
    {
        public long EmpresaId { get; set; }
        public long SucursalId { get; set; }
        public long ProductoId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal StockMinimo { get; set; }
    }

    /// <summary>DTO de stock con nombre de producto/sucursal.</summary>
    public class StockDTO : BaseObject
    {
        public long EmpresaId { get; set; }
        public long SucursalId { get; set; }
        public string SucursalNombre { get; set; }
        public long ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal StockMinimo { get; set; }
        public bool BajoMinimo { get; set; }
    }

    /// <summary>DTO de movimiento de inventario.</summary>
    public class StockMovimientoDTO : BaseObject
    {
        public long EmpresaId { get; set; }
        public long SucursalId { get; set; }
        public string SucursalNombre { get; set; }
        public long ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public string TipoMovimiento { get; set; }
        public decimal Cantidad { get; set; }
        public decimal ExistenciaAnterior { get; set; }
        public decimal ExistenciaNueva { get; set; }
        public string Motivo { get; set; }
        public long? ReferenciaId { get; set; }
    }
}
