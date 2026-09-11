using System;

namespace PuntoDeVenta.Entities.Models
{
    public class Empresa
    {
        public long Id { get; set; }
        public string NombreComercial { get; set; }
        public string RazonSocial { get; set; }
        public string RFC { get; set; }
        public string Responsable { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string CodigoPostal { get; set; }
        public string Telefono { get; set; }
        public string CorreoContacto { get; set; }
        public DateTime FechaVigenciaInicio { get; set; }
        public DateTime FechaVigenciaFin { get; set; }
        public bool EsPeriodoPrueba { get; set; }
        public bool Estatus { get; set; }
        public string CreadoPor { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string ModificadoPor { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string LogoUrl { get; set; }
    }
}
