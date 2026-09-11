namespace PuntoDeVentaEntities.Seguridad
{
    /// <summary>Permisos de un rol por página (pantalla de administración de permisos).</summary>
    public class PermisosViewModel
    {
        public long PaginaId { get; set; }
        public string PaginaNombre { get; set; }
        public string Direccion { get; set; }
        public bool PuedeLeer { get; set; }
        public bool PuedeCrear { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeEliminar { get; set; }
        public bool PuedeExportar { get; set; }
    }
}
