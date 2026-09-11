namespace PuntoDeVentaEntities.Autenticacion
{
    public class Usuario : BaseObject
    {
        public long EmpresaId { get; set; }
        public string NombreUsuario { get; set; }

        /// <summary>Contraseña en claro; solo se usa como transporte en el login (nunca se persiste).</summary>
        public string Contrasena { get; set; }

        public string ContrasenaHash { get; set; }
        public string ContrasenaSalt { get; set; }
        public int ContrasenaIteraciones { get; set; }

        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string ImagenPerfil { get; set; }
    }
}
