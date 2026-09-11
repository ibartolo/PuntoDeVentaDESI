using System;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaWebApi.DAL;
using Serilog;

namespace PuntoDeVentaWebApi.Services
{
    /// <summary>Servicio de alta de Empresa (registro público de nuevas empresas).</summary>
    public class EmpresaService
    {
        private readonly DbWrapper _dbWrapper;

        public EmpresaService()
        {
            _dbWrapper = new DbWrapper();
        }

        public ModelResponse<Empresa> GuardarEmpresa(Empresa empresa)
        {
            try
            {
                Log.Information("EmpresaService.GuardarEmpresa");

                if (empresa == null)
                {
                    throw new ArgumentException("La empresa es requerida.");
                }

                if (string.IsNullOrWhiteSpace(empresa.NombreComercial))
                {
                    throw new ArgumentException("El Nombre Comercial es requerido.");
                }

                if (string.IsNullOrWhiteSpace(empresa.RazonSocial))
                {
                    throw new ArgumentException("La Razón Social es requerida.");
                }

                if (string.IsNullOrWhiteSpace(empresa.RFC))
                {
                    throw new ArgumentException("El RFC es requerido.");
                }

                if (string.IsNullOrWhiteSpace(empresa.Responsable))
                {
                    throw new ArgumentException("El Responsable es requerido.");
                }

                if (string.IsNullOrWhiteSpace(empresa.Direccion))
                {
                    throw new ArgumentException("La Dirección es requerida.");
                }

                if (string.IsNullOrWhiteSpace(empresa.CorreoContacto))
                {
                    throw new ArgumentException("El Correo de Contacto es requerido.");
                }

                return _dbWrapper.GuardarEmpresa(empresa);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Error de validación en GuardarEmpresa");
                return new ModelResponse<Empresa> { IsSuccess = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en GuardarEmpresa");
                return new ModelResponse<Empresa> { IsSuccess = false, Message = "Ocurrió un error al registrar la empresa." };
            }
        }
    }
}
