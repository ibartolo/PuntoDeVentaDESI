using System;

namespace PuntoDeVentaEntities.Seguridad
{
    /// <summary>Token de un solo uso para restablecer la contraseña.</summary>
    public class TokenRecuperacion : BaseObject
    {
        public long UsuarioId { get; set; }
        public string Token { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public bool Usado { get; set; }
    }
}
