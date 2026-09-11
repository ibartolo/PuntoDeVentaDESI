using System;

namespace PuntoDeVentaEntities
{
    /// <summary>
    /// Clase base para todas las entidades de dominio.
    /// Incluye Id y las 4 columnas de auditoría + borrado lógico (Estatus).
    /// </summary>
    public class BaseObject
    {
        public long Id { get; set; }
        public string CreadoPor { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string ModificadoPor { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool Estatus { get; set; }
    }
}
