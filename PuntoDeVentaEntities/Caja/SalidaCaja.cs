using System;

namespace PuntoDeVentaEntities.Caja
{
    /// <summary>Salida de dinero de la caja chica (sale de los ingresos, limite 50%).</summary>
    public class SalidaCaja : BaseObject
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
