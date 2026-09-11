namespace PuntoDeVentaEntities.Catalogos
{
    /// <summary>Proveedor de la empresa. Un producto puede tener varios proveedores.</summary>
    public class Proveedor : BaseObject
    {
        public long EmpresaId { get; set; }
        public string Nombre { get; set; }
        public string Contacto { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
    }

    /// <summary>DTO de proveedor.</summary>
    public class ProveedorDTO : BaseObject
    {
        public long EmpresaId { get; set; }
        public string Nombre { get; set; }
        public string Contacto { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
    }
}
