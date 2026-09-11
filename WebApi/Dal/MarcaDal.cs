using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using PuntoDeVenta.Entities.Models;

namespace PuntoDeVenta.WebApi.Dal
{
    internal class MarcaDal
    {
        private const int DuplicadoSqlErrorNumber = 2601;
        private const int DuplicadoConstraintSqlErrorNumber = 2627;
        private readonly string _connectionString;

        public MarcaDal()
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

        public List<Marca> Listar(long empresaId)
        {
            var marcas = new List<Marca>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_Marca_Listar", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@EmpresaId", SqlDbType.BigInt).Value = empresaId;

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        marcas.Add(MapearMarca(reader));
                    }
                }
            }

            return marcas;
        }

        public Marca Consultar(long empresaId, long marcaId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_Marca_Consultar", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@EmpresaId", SqlDbType.BigInt).Value = empresaId;
                command.Parameters.Add("@MarcaId", SqlDbType.BigInt).Value = marcaId;

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return MapearMarca(reader);
                }
            }
        }

        public long Insertar(long empresaId, string nombre, string descripcion, string actor)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_Marca_Insertar", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@EmpresaId", SqlDbType.BigInt).Value = empresaId;
                command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = nombre;
                command.Parameters.Add("@Descripcion", SqlDbType.NVarChar, -1).Value = (object)descripcion ?? DBNull.Value;
                command.Parameters.Add("@Actor", SqlDbType.NVarChar, 25).Value = actor;

                connection.Open();

                try
                {
                    var scalar = command.ExecuteScalar();
                    return Convert.ToInt64(scalar);
                }
                catch (SqlException ex) when (EsDuplicadoActivoPorEmpresa(ex))
                {
                    throw new InvalidOperationException("Ya existe una Marca activa con ese nombre para esta empresa.", ex);
                }
            }
        }

        public int Actualizar(long empresaId, long marcaId, string nombre, string descripcion, string actor)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_Marca_Actualizar", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@EmpresaId", SqlDbType.BigInt).Value = empresaId;
                command.Parameters.Add("@MarcaId", SqlDbType.BigInt).Value = marcaId;
                command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = nombre;
                command.Parameters.Add("@Descripcion", SqlDbType.NVarChar, -1).Value = (object)descripcion ?? DBNull.Value;
                command.Parameters.Add("@Actor", SqlDbType.NVarChar, 25).Value = actor;

                connection.Open();

                try
                {
                    var scalar = command.ExecuteScalar();
                    return scalar == null ? 0 : Convert.ToInt32(scalar);
                }
                catch (SqlException ex) when (EsDuplicadoActivoPorEmpresa(ex))
                {
                    throw new InvalidOperationException("Ya existe una Marca activa con ese nombre para esta empresa.", ex);
                }
            }
        }

        public int EliminarLogico(long empresaId, long marcaId, string actor)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_Marca_EliminarLogico", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@EmpresaId", SqlDbType.BigInt).Value = empresaId;
                command.Parameters.Add("@MarcaId", SqlDbType.BigInt).Value = marcaId;
                command.Parameters.Add("@Actor", SqlDbType.NVarChar, 25).Value = actor;

                connection.Open();

                var scalar = command.ExecuteScalar();
                return scalar == null ? 0 : Convert.ToInt32(scalar);
            }
        }

        private static Marca MapearMarca(IDataRecord reader)
        {
            return new Marca
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                EmpresaId = reader.GetInt64(reader.GetOrdinal("EmpresaId")),
                Nombre = reader["Nombre"] as string,
                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader["Descripcion"] as string,
                Estatus = reader.GetBoolean(reader.GetOrdinal("Estatus")),
                CreadoPor = reader["CreadoPor"] as string,
                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                ModificadoPor = reader.IsDBNull(reader.GetOrdinal("ModificadoPor")) ? null : reader["ModificadoPor"] as string,
                FechaModificacion = reader.IsDBNull(reader.GetOrdinal("FechaModificacion"))
                    ? (DateTime?)null
                    : reader.GetDateTime(reader.GetOrdinal("FechaModificacion"))
            };
        }

        private static bool EsDuplicadoActivoPorEmpresa(SqlException ex)
        {
            return ex.Number == DuplicadoSqlErrorNumber || ex.Number == DuplicadoConstraintSqlErrorNumber;
        }
    }
}
