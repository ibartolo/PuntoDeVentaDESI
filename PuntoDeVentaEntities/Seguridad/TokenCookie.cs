namespace PuntoDeVentaEntities.Seguridad
{
    /// <summary>DTO que viaja dentro de la cookie FormsAuth del front.</summary>
    public class TokenCookie
    {
        public Token Token { get; set; }
        public long UserID { get; set; }
        public long EmpresaID { get; set; }
        public long SucursalID { get; set; }
        public string UserName { get; set; }
        public string ProfileImage { get; set; }
        public string UserAvatar { get; set; }
    }
}
