using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace PuntoDeVentaWebApi.DAL
{
    /// <summary>
    /// Wrapper concreto. Lee la cadena de conexión de la variable de entorno sConSql
    /// y, si no existe, de connectionStrings["cCon"].
    /// </summary>
    public partial class DbWrapper : BaseDbWrapper
    {
        private readonly string _connectionString;

        protected override string SQLConnectionString
        {
            get { return _connectionString; }
        }

        protected override TimeSpan SQLCommandTimeOut
        {
            get { return TimeSpan.FromSeconds(15); }
        }

        public DbWrapper()
        {
            var env = Environment.GetEnvironmentVariable("sConSql");
            var cfg = ConfigurationManager.ConnectionStrings["cCon"]?.ConnectionString;
            _connectionString = !string.IsNullOrWhiteSpace(env) ? env : cfg;

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException("Cadena de conexión no configurada (sConSql o cCon).");
            }
        }

        /// <summary>
        /// Mapea por reflexión cada columna del reader a una propiedad pública del tipo T
        /// (match case-insensitive por nombre). DBNull -> null / default.
        /// </summary>
        public T LlenarEntidad<T>(IDataReader reader) where T : class, new()
        {
            var obj = new T();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var property = properties.FirstOrDefault(p => string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase));
                if (property == null || !property.CanWrite)
                {
                    continue;
                }

                var value = reader.GetValue(i);
                if (value == null || value == DBNull.Value)
                {
                    if (Nullable.GetUnderlyingType(property.PropertyType) != null || !property.PropertyType.IsValueType)
                    {
                        property.SetValue(obj, null);
                    }

                    continue;
                }

                try
                {
                    var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    if (targetType.IsEnum)
                    {
                        property.SetValue(obj, Enum.Parse(targetType, value.ToString(), true));
                    }
                    else if (targetType == typeof(bool))
                    {
                        property.SetValue(obj, Convert.ToBoolean(value));
                    }
                    else
                    {
                        property.SetValue(obj, Convert.ChangeType(value, targetType));
                    }
                }
                catch
                {
                    // columna no mapeable -> se ignora
                }
            }

            return obj;
        }

        /// <summary>Refleja todas las propiedades públicas de T a SqlParameter("@Prop", value).</summary>
        public List<SqlParameter> ObtenerParametrosSQL<T>(T o)
        {
            var parameters = new List<SqlParameter>();
            if (o == null)
            {
                return parameters;
            }

            foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!property.CanRead)
                {
                    continue;
                }

                var value = property.GetValue(o);
                parameters.Add(new SqlParameter("@" + property.Name, value ?? (object)DBNull.Value) { IsNullable = true });
            }

            return parameters;
        }
    }
}
