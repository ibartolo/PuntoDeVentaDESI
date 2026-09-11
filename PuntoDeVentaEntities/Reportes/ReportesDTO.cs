using System;

namespace PuntoDeVentaEntities.Reportes
{
    /// <summary>Reporte: ventas por periodo.</summary>
    public class ReporteVentasPeriodoDTO
    {
        public DateTime Fecha { get; set; }
        public string Folio { get; set; }
        public string SucursalNombre { get; set; }
        public string CajeroNombre { get; set; }
        public string MetodoPago { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
    }

    /// <summary>Reporte: ventas por sucursal.</summary>
    public class ReporteVentasSucursalDTO
    {
        public long SucursalId { get; set; }
        public string SucursalNombre { get; set; }
        public int NumeroVentas { get; set; }
        public decimal Total { get; set; }
    }

    /// <summary>Reporte: ventas por cajero.</summary>
    public class ReporteVentasCajeroDTO
    {
        public long UsuarioId { get; set; }
        public string CajeroNombre { get; set; }
        public int NumeroVentas { get; set; }
        public decimal Total { get; set; }
    }

    /// <summary>Reporte: utilidad por producto.</summary>
    public class ReporteUtilidadDTO
    {
        public long ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public decimal CantidadVendida { get; set; }
        public decimal TotalVenta { get; set; }
        public decimal TotalCosto { get; set; }
        public decimal Utilidad { get; set; }
    }

    /// <summary>Reporte: productos mas vendidos.</summary>
    public class ReporteMasVendidosDTO
    {
        public long ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public decimal CantidadVendida { get; set; }
        public decimal TotalVenta { get; set; }
    }
}
