namespace PuntoDeVentaEntities.Seguridad
{
    /// <summary>Permiso de rol/página con columnas de join para las pantallas de administración.</summary>
    public class RolPaginaAccionDTO : RolPaginaAccion
    {
        public string PaginaNombre { get; set; }
        public string Direccion { get; set; }
    }
}
