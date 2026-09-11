using System;

namespace PuntoDeVentaEntities.Seguridad
{
    /// <summary>Espejo del JSON OAuth (nombres en minúsculas) + expiración local.</summary>
    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
