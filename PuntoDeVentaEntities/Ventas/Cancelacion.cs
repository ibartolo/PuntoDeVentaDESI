using System;
using System.Collections.Generic;

namespace PuntoDeVentaEntities.Ventas
{
    /// <summary>Cancelacion/devolucion total o parcial de una venta, con autorizacion.</summary>
    public class Cancelacion : BaseObject
    {
        public long EmpresaId { get; set; }
        public long SucursalId { get; set; }
        public long VentaId { get; set; }
        public string Tipo { get; set; }
        public string Motivo { get; set; }
        public string UsuarioCancela { get; set; }
        public string UsuarioAutoriza { get; set; }
        public DateTime FechaCancelacion { get; set; }
        public decimal TotalReembolsado { get; set; }
        public string MetodoReembolso { get; set; }
    }

    /// <summary>DTO de cancelacion con su detalle.</summary>
    public class CancelacionDTO : BaseObject
    {
        public long EmpresaId { get; set; }
        public long SucursalId { get; set; }
        public long VentaId { get; set; }
        public string VentaFolio { get; set; }
        public string Tipo { get; set; }
        public string Motivo { get; set; }
        public string UsuarioCancela { get; set; }
        public string UsuarioAutoriza { get; set; }
        public DateTime FechaCancelacion { get; set; }
        public decimal TotalReembolsado { get; set; }
        public string MetodoReembolso { get; set; }
        public List<DevolucionDetalleDTO> Detalle { get; set; }
    }

    /// <summary>DTO de detalle de devolucion.</summary>
    public class DevolucionDetalleDTO : BaseObject
    {
        public long CancelacionId { get; set; }
        public long VentaDetalleId { get; set; }
        public long ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Importe { get; set; }
        public bool RegresaAStock { get; set; }
    }
}
