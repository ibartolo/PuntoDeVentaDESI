namespace PuntoDeVentaEntities.Autenticacion
{
    /// <summary>Usuario con columnas de join/display para vistas y respuestas de la API.</summary>
    public class UsuarioDTO : Usuario
    {
        public string EmpresaNombre { get; set; }
        public long? SucursalId { get; set; }
        public string SucursalNombre { get; set; }
    }
}
