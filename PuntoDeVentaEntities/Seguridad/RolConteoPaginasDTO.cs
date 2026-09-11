namespace PuntoDeVentaEntities.Seguridad
{
    /// <summary>Conteo de páginas asignadas a un rol (pantalla de roles).</summary>
    public class RolConteoPaginasDTO
    {
        public long RolId { get; set; }
        public int TotalPaginas { get; set; }
    }
}
