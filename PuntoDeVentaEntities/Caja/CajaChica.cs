using System;

namespace PuntoDeVentaEntities.Caja
{
    /// <summary>Caja chica por usuario + sucursal. Debe permanecer intacta; se cierra con corte.</summary>
    public class CajaChica : BaseObject
    {
        public long EmpresaId { get; set; }
        public long SucursalId { get; set; }
        public long UsuarioId { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal IngresosTotales { get; set; }
        public decimal SalidasTotales { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string Estado { get; set; }
    }

    /// <summary>DTO de caja chica.</summary>
    public class CajaChicaDTO : BaseObject
    {
        public long EmpresaId { get; set; }
        public long SucursalId { get; set; }
        public string SucursalNombre { get; set; }
        public long UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal IngresosTotales { get; set; }
        public decimal SalidasTotales { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string Estado { get; set; }
    }

    /// <summary>DTO de salida de caja.</summary>
    public class SalidaCajaDTO : BaseObject
    {
        public long EmpresaId { get; set; }
        public long CajaChicaId { get; set; }
        public decimal Monto { get; set; }
        public string Comentario { get; set; }
        public DateTime FechaHora { get; set; }
        public bool Justificada { get; set; }
        public string EvidenciaUrl { get; set; }
        public string TipoSalida { get; set; }
    }
}
