using System;

namespace PuntoDeVenta.MVC.Models
{
    public class MarcaViewModel
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Estatus { get; set; }
        public string CreadoPor { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string ModificadoPor { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
