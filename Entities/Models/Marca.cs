using System;

namespace PuntoDeVenta.Entities.Models
{
    public class Marca
    {
        public long Id { get; set; }
        public long EmpresaId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Estatus { get; set; }

        // Auditoría (exactamente 4 columnas requeridas)
        public string CreadoPor { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string ModificadoPor { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
