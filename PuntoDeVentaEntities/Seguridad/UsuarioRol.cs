namespace PuntoDeVentaEntities.Seguridad
{
    /// <summary>Relación N:M entre usuarios y roles.</summary>
    public class UsuarioRol : BaseObject
    {
        public long UsuarioId { get; set; }
        public long RolId { get; set; }
    }
}
