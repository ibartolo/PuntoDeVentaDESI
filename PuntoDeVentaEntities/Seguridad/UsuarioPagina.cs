namespace PuntoDeVentaEntities.Seguridad
{
    /// <summary>Acceso directo (excepción) de un usuario a una página, además de sus roles.</summary>
    public class UsuarioPagina : BaseObject
    {
        public long? UsuarioId { get; set; }
        public long? PaginaId { get; set; }
    }
}
