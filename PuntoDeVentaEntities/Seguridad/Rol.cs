namespace PuntoDeVentaEntities.Seguridad
{
    /// <summary>
    /// Rol de seguridad de una empresa. Los roles base vienen precargados y la empresa
    /// puede editarlos o ampliarlos (Propuesta: "Roles básicos precargados, editables y
    /// ampliables por la empresa").
    /// </summary>
    public class Rol : BaseObject
    {
        public long EmpresaId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        /// <summary>Habilita acciones de autorización (cancelaciones, ajustes) dentro de los módulos.</summary>
        public bool PuedeAutorizar { get; set; }
    }
}
