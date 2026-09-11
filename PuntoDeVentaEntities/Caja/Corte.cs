using System;
using System.Collections.Generic;

namespace PuntoDeVentaEntities.Caja
{
    /// <summary>Corte de caja: cierra la caja chica y resume ventas por metodo de pago.</summary>
    public class Corte : BaseObject
    {
        public long EmpresaId { get; set; }
        public long SucursalId { get; set; }
        public long UsuarioId { get; set; }
        public long CajaChicaId { get; set; }
        public DateTime FechaCorte { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal TotalEfectivo { get; set; }
        public decimal TotalTarjeta { get; set; }
        public decimal TotalTransferencia { get; set; }
        public decimal Salidas { get; set; }
        public decimal EfectivoEsperado { get; set; }
        public decimal EfectivoContado { get; set; }
        public decimal Diferencia { get; set; }
        public string Observaciones { get; set; }
    }

    /// <summary>DTO de corte con su detalle.</summary>
    public class CorteDTO : BaseObject
    {
        public long EmpresaId { get; set; }
        public long SucursalId { get; set; }
        public string SucursalNombre { get; set; }
        public long UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public long CajaChicaId { get; set; }
        public DateTime FechaCorte { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal TotalEfectivo { get; set; }
        public decimal TotalTarjeta { get; set; }
        public decimal TotalTransferencia { get; set; }
        public decimal Salidas { get; set; }
        public decimal EfectivoEsperado { get; set; }
        public decimal EfectivoContado { get; set; }
        public decimal Diferencia { get; set; }
        public string Observaciones { get; set; }
        public List<CorteDetalleDTO> Detalle { get; set; }
    }

    /// <summary>DTO de detalle de corte.</summary>
    public class CorteDetalleDTO : BaseObject
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
