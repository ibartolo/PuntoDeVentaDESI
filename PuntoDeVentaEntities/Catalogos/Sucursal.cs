namespace PuntoDeVentaEntities.Catalogos
{
    /// <summary>Sucursal de una empresa (multi-sucursal; el stock es independiente por sucursal).</summary>
    public class Sucursal : BaseObject
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
