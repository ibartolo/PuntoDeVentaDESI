namespace PuntoDeVentaEntities.Seguridad
{
    /// <summary>Página/menú del sistema sobre la que se otorgan permisos por rol.</summary>
    public class Pagina : BaseObject
    {
        public string Nombre { get; set; }
        public string NombreVisible { get; set; }
        public string Descripcion { get; set; }

        /// <summary>Menu o SubMenu.</summary>
        public string Tipo { get; set; }

        public string Direccion { get; set; }

        /// <summary>Página padre para la jerarquía del menú.</summary>
        public long? PermisosPadreId { get; set; }

        /// <summary>Ícono FontAwesome (ej. fa-users).</summary>
        public string Logo { get; set; }

        /// <summary>Orden de visualización.</summary>
        public int OrdenB { get; set; }
    }
}
