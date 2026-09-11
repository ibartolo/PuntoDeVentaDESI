using System.Collections.Generic;

namespace PuntoDeVentaEntities.Seguridad
{
    /// <summary>Permiso de una página para un rol (usado al guardar permisos masivos).</summary>
    public class PermisoRequest
    {
        public long PaginaId { get; set; }
        public bool PuedeLeer { get; set; }
        public bool PuedeCrear { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeEliminar { get; set; }
        public bool PuedeExportar { get; set; }
    }

    /// <summary>Consulta de validación de un permiso concreto (página + acción).</summary>
    public class ValidarPermisoRequest
    {
        public string NombrePagina { get; set; }
        public string Accion { get; set; }
    }

    /// <summary>Alta/edición de un permiso rol-página.</summary>
    public class GuardarPermisosRequest
    {
        public long RolId { get; set; }
        public long PaginaId { get; set; }
        public bool PuedeLeer { get; set; }
        public bool PuedeCrear { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeEliminar { get; set; }
        public bool PuedeExportar { get; set; }
    }

    /// <summary>Guardado masivo de los permisos de un rol.</summary>
    public class GuardarPermisosMasivoRequest
    {
        public long RolId { get; set; }
        public List<PermisoRequest> Permisos { get; set; }
    }
}
