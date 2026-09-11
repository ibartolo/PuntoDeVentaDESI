using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PuntoDeVentaWebApi.DAL
{
    /// <summary>
    /// Base abstracta de acceso a datos contra SQL Server (solo stored procedures).
    /// Mantiene una transacción "ambiente" sobre una única conexión para evitar MSDTC.
    /// </summary>
    public abstract class BaseDbWrapper : IDisposable
    {
        protected abstract string SQLConnectionString { get; }
        protected abstract TimeSpan SQLCommandTimeOut { get; }

        private SqlConnection _connection;
        private SqlTransaction _transaction;

        protected SqlConnection GetOpenConnection()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(SQLConnectionString);
                _connection.Open();
            }
            else if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            return _connection;
        }

        public void BeginTransaction()
        {
            var connection = GetOpenConnection();
            if (_transaction == null)
            {
                _transaction = connection.BeginTransaction();
            }
        }

        public void CommitTransaction()
        {
            if (_transaction == null)
            {
                return;
            }

            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }

        public void RollbackTransaction()
        {
            if (_transaction == null)
            {
                return;
            }

            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }

        private SqlCommand CreateCommand(string cmdText, CommandType cmdType, IEnumerable<SqlParameter> pars)
        {
            var connection = GetOpenConnection();
            var command = new SqlCommand(cmdText, connection)
            {
                CommandType = cmdType,
                CommandTimeout = (int)SQLCommandTimeOut.TotalSeconds
            };

            if (_transaction != null)
            {
                command.Transaction = _transaction;
            }

            if (pars != null)
            {
                foreach (var parameter in pars)
                {
                    if (parameter.Value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }

                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

        protected object ExecuteScalar(string cmdText, CommandType cmdType = CommandType.StoredProcedure, IEnumerable<SqlParameter> pars = null)
        {
            using (var command = CreateCommand(cmdText, cmdType, pars))
            {
                return command.ExecuteScalar();
            }
        }

        protected int ExecuteNonQuery(string cmdText, CommandType cmdType = CommandType.StoredProcedure, IEnumerable<SqlParameter> pars = null)
        {
            using (var command = CreateCommand(cmdText, cmdType, pars))
            {
                return command.ExecuteNonQuery();
            }
        }

        protected T GetObject<T>(string cmdText, Func<IDataReader, T> mapper, CommandType cmdType = CommandType.StoredProcedure, IEnumerable<SqlParameter> pars = null)
        {
            using (var command = CreateCommand(cmdText, cmdType, pars))
            using (var reader = command.ExecuteReader())
            {
                return reader.Read() ? mapper(reader) : default(T);
            }
        }

        protected IEnumerable<T> GetObjects<T>(string cmdText, Func<IDataReader, T> mapper, CommandType cmdType = CommandType.StoredProcedure, IEnumerable<SqlParameter> pars = null)
        {
            var list = new List<T>();
            using (var command = CreateCommand(cmdText, cmdType, pars))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(mapper(reader));
                }
            }

            return list;
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                try { _transaction.Dispose(); } catch { }
                _transaction = null;
            }

            if (_connection != null)
            {
                try { _connection.Dispose(); } catch { }
                _connection = null;
            }
        }
    }
}
