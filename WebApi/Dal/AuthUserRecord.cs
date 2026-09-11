namespace PuntoDeVenta.WebApi.Dal
{
    internal class AuthUserRecord
    {
        public long Id { get; set; }
        public long EmpresaId { get; set; }
        public string NombreUsuario { get; set; }
        public string ContrasenaHash { get; set; }
        public string ContrasenaSalt { get; set; }
        public int ContrasenaIteraciones { get; set; }
        public bool Estatus { get; set; }
    }
}
