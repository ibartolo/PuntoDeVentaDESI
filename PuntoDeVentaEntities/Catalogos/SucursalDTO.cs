namespace PuntoDeVentaEntities.Catalogos
{
    /// <summary>DTO de sucursal (Sucursal.cs ya existe).</summary>
    public class SucursalDTO : BaseObject
    {
        public long EmpresaId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Calle { get; set; }
        public string Ciudad { get; set; }
        public string Colonia { get; set; }
        public string CodigoPostal { get; set; }
    }
}
