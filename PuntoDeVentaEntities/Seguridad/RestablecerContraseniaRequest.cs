namespace PuntoDeVentaEntities.Seguridad
{
    /// <summary>Solicitud de restablecimiento de contraseña a partir de un token de recuperación.</summary>
    public class RestablecerContraseniaRequest
    {
        public string Token { get; set; }
        public string NuevaContrasena { get; set; }
    }
}
