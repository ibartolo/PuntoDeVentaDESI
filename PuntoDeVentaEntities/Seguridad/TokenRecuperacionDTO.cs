namespace PuntoDeVentaEntities.Seguridad
{
    /// <summary>Token de recuperación con los datos de contacto del usuario (para el correo).</summary>
    public class TokenRecuperacionDTO : TokenRecuperacion
    {
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
    }
}
