namespace PuntoDeVentaEntities.Catalogos
{
    /// <summary>Categoria jerarquica tipo supermercado (puede tener categoria padre y area).</summary>
    public class Categoria : BaseObject
    {
        public long EmpresaId { get; set; }
        public long? CategoriaPadreId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Area { get; set; }
        public int Orden { get; set; }
    }

    /// <summary>DTO de categoria con el nombre del padre resuelto.</summary>
    public class CategoriaDTO : BaseObject
    {
        public long EmpresaId { get; set; }
        public long? CategoriaPadreId { get; set; }
        public string CategoriaPadreNombre { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Area { get; set; }
        public int Orden { get; set; }
    }
}
