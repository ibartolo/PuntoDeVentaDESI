namespace PuntoDeVentaEntities.Catalogos
{
    /// <summary>Cliente de la empresa. Incluye el cliente especial "Publico General".</summary>
    public class Cliente : BaseObject
    {
        public long EmpresaId { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public bool EsPublicoGeneral { get; set; }
    }

    /// <summary>DTO de cliente.</summary>
    public class ClienteDTO : BaseObject
    {
        public long EmpresaId { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public bool EsPublicoGeneral { get; set; }
    }
}
