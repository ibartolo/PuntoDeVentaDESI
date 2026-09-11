using System;
using System.Collections.Generic;

namespace PuntoDeVentaEntities.Compras
{
    /// <summary>Compra a proveedor (historial de compras). Sin orden de compra formal.</summary>
    public class Compra : BaseObject
    {
        public long EmpresaId { get; set; }
        public long SucursalId { get; set; }
        public long ProveedorId { get; set; }
        public string Folio { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public string Observaciones { get; set; }
    }

    /// <summary>DTO de compra con datos resueltos y su detalle.</summary>
    public class CompraDTO : BaseObject
    {
        public long EmpresaId { get; set; }
        public long SucursalId { get; set; }
        public string SucursalNombre { get; set; }
        public long ProveedorId { get; set; }
        public string ProveedorNombre { get; set; }
        public string Folio { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public string Observaciones { get; set; }
        public List<CompraDetalleDTO> Detalle { get; set; }
    }

    /// <summary>DTO de detalle de compra.</summary>
    public class CompraDetalleDTO : BaseObject
    {
        public long CompraId { get; set; }
        public long ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal Importe { get; set; }
    }
}
