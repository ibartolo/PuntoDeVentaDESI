using System.ComponentModel.DataAnnotations;

namespace PuntoDeVenta.MVC.Models
{
    public class MarcaUpsertViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }
    }
}
