using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace PuntoDeVenta.WebApi.Dal
{
    internal class AuthDal
    {
        private readonly string _connectionString;

        public AuthDal()
        {
            _connectionString = Environment.GetEnvironmentVariable("sConSql");
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                _connectionString = ConfigurationManager.ConnectionStrings["cCon"]?.ConnectionString;
            }

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException("Cadena de conexión no configurada (sConSql o cCon).");
            }
        }

        public AuthUserRecord ObtenerUsuarioParaLogin(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                return null;
            }

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_Usuario_ObtenerParaLogin", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar, 25).Value = nombreUsuario.Trim();

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new AuthUserRecord
                    {
                        Id = reader.GetInt64(reader.GetOrdinal("Id")),
                        EmpresaId = reader.GetInt64(reader.GetOrdinal("EmpresaId")),
                        NombreUsuario = reader["NombreUsuario"] as string,
                        ContrasenaHash = reader["ContrasenaHash"] as string,
                        ContrasenaSalt = reader["ContrasenaSalt"] as string,
                        ContrasenaIteraciones = reader.GetInt32(reader.GetOrdinal("ContrasenaIteraciones")),
                        Estatus = reader.GetBoolean(reader.GetOrdinal("Estatus"))
                    };
                }
            }
        }
    }
}
