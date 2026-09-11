using System;
using System.Data;
using System.Data.SqlClient;
using PuntoDeVentaEntities.Catalogos;
using PuntoDeVentaEntities.Seguridad;
using Serilog;

namespace PuntoDeVentaWebApi.DAL
{
    public partial class DbWrapper
    {
        /// <summary>
        /// Registra una nueva empresa (alta pública) y devuelve la entidad con su Id asignado.
        /// La vigencia inicial se fija en el SP (30 días de periodo de prueba).
        /// </summary>
        public ModelResponse<Empresa> GuardarEmpresa(Empresa empresa)
        {
            var mr = new ModelResponse<Empresa>();
            try
            {
                if (empresa == null)
                {
                    mr.IsSuccess = false;
                    mr.Message = "La empresa es requerida.";
                    return mr;
                }

                var actor = (empresa.CreadoPor ?? "registro").Trim();
                if (actor.Length > 25)
                {
                    actor = actor.Substring(0, 25);
                }

                var pars = new[]
                {
                    new SqlParameter("@NombreComercial", (object)empresa.NombreComercial ?? DBNull.Value),
                    new SqlParameter("@RazonSocial", (object)empresa.RazonSocial ?? DBNull.Value),
                    new SqlParameter("@RFC", (object)empresa.RFC ?? DBNull.Value),
                    new SqlParameter("@Responsable", (object)empresa.Responsable ?? DBNull.Value),
                    new SqlParameter("@Direccion", (object)empresa.Direccion ?? DBNull.Value),
                    new SqlParameter("@Ciudad", (object)empresa.Ciudad ?? DBNull.Value),
                    new SqlParameter("@Estado", (object)empresa.Estado ?? DBNull.Value),
                    new SqlParameter("@CodigoPostal", (object)empresa.CodigoPostal ?? DBNull.Value),
                    new SqlParameter("@Telefono", (object)empresa.Telefono ?? DBNull.Value),
                    new SqlParameter("@CorreoContacto", (object)empresa.CorreoContacto ?? DBNull.Value),
                    new SqlParameter("@Actor", actor)
                };

                var resultado = ExecuteScalar("sp_Empresa_Guardar", CommandType.StoredProcedure, pars);
                empresa.Id = resultado == null || resultado == DBNull.Value ? 0L : Convert.ToInt64(resultado);

                if (empresa.Id <= 0)
                {
                    mr.IsSuccess = false;
                    mr.Message = "No se pudo registrar la empresa.";
                    return mr;
                }

                mr.IsSuccess = true;
                mr.Response = empresa;
                mr.Message = "Empresa registrada correctamente";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al registrar empresa {NombreComercial}", empresa?.NombreComercial);
                mr.IsSuccess = false;
                mr.Message = "Ocurrió un error al registrar la empresa.";
            }

            return mr;
        }
    }
}
