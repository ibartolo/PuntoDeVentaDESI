using System;

namespace PuntoDeVenta.MVC.Models
{
    public class ServerSessionToken
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public DateTime ExpirationUtc { get; set; }
        public string Subject { get; set; }
        public long EmpresaId { get; set; }
    }
}
