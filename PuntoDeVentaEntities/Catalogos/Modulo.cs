namespace PuntoDeVentaEntities.Catalogos
{
    /// <summary>
    /// Módulo del sistema (tabla fija): nombre y URL parcial. Se cargan en el menú
    /// según el rol del usuario (Propuesta: "Módulos del sistema").
    /// </summary>
    public class Modulo : BaseObject
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        /// <summary>URL parcial del módulo.</summary>
        public string Direccion { get; set; }

        /// <summary>Ícono FontAwesome (ej. fa-cash-register).</summary>
        public string Logo { get; set; }

        /// <summary>Orden de visualización.</summary>
        public int OrdenB { get; set; }
    }
}
