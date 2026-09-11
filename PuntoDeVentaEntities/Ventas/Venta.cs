using System;
using System.Collections.Generic;

namespace PuntoDeVentaEntities.Ventas
{
    /// <summary>Venta POS. Un solo metodo de pago por venta (efectivo, transferencia o tarjeta).</summary>
    public class Venta : BaseObject
    {
        public long EmpresaId { get; set; }
        public long SucursalId { get; set; }
        public long UsuarioId { get; set; }
        public long? ClienteId { get; set; }
        public long? CajaChicaId { get; set; }
        public string Folio { get; set; }
        public DateTime FechaVenta { get; set; }
        public string MetodoPago { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
    }

    /// <summary>DTO de venta con datos resueltos y detalle.</summary>
    public class VentaDTO : BaseObject
    {
        public long EmpresaId { get; set; }
        public long SucursalId { get; set; }
        public string SucursalNombre { get; set; }
        public long UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public long? ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public long? CajaChicaId { get; set; }
        public string Folio { get; set; }
        public DateTime FechaVenta { get; set; }
        public string MetodoPago { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public List<VentaDetalleDTO> Detalle { get; set; }
    }

    /// <summary>DTO de detalle de venta.</summary>
    public class VentaDetalleDTO : BaseObject
    {
        public long VentaId { get; set; }
        public long ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Importe { get; set; }
    }
}
