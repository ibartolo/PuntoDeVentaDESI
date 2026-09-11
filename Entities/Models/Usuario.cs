using System;

namespace PuntoDeVenta.Entities.Models
{
    public class Usuario
    {
        public long Id { get; set; }
        public long EmpresaId { get; set; }
        public string NombreUsuario { get; set; }
        public string ContrasenaHash { get; set; }
        public string ContrasenaSalt { get; set; }
        public int ContrasenaIteraciones { get; set; }
        public bool Estatus { get; set; }

        // Auditoría (exactamente 4 columnas requeridas)
        public string CreadoPor { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string ModificadoPor { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
