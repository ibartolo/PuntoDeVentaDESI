using System;

namespace PuntoDeVentaEntities.Catalogos
{
    /// <summary>Historial de precios por producto; solo uno activo a la vez (precio global).</summary>
    public class Precio : BaseObject
    {
        public long EmpresaId { get; set; }
        public long ProductoId { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVentaSugerido { get; set; }
        public decimal PrecioVenta { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }

    /// <summary>DTO de precio con el nombre del producto resuelto.</summary>
    public class PrecioDTO : BaseObject
    {
        public long EmpresaId { get; set; }
        public long ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVentaSugerido { get; set; }
        public decimal PrecioVenta { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
