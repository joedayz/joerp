using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace JOERP.DataAccess
{
    public abstract class BaseRepository<T> : IRepository<T>
        where T : class, new()
    {
        #region Atributos

        private const int CommandTimeout = 6000;
        protected readonly Database Database = DatabaseFactory.CreateDatabase();
        private readonly string _countStoredProcedure = string.Format("usp_Count_{0}", typeof(T).Name);
        private readonly string _deleteStoredProcedure = string.Format("usp_Delete_{0}", typeof(T).Name);
        private readonly string _insertStoredProcedure = string.Format("usp_Insert_{0}", typeof(T).Name);
        private readonly string _selectPagedStoredProcedure = string.Format("usp_SelectPaged_{0}", typeof(T).Name);
        private readonly string _selectStoredProcedure = string.Format("usp_Select_{0}", typeof(T).Name);
        private readonly string _singleStoredProcedure = string.Format("usp_Single_{0}", typeof(T).Name);
        private readonly string _updateStoredProcedure = string.Format("usp_Update_{0}", typeof(T).Name);

        #endregion Atributos

        #region Miembros de IRepository<T>

        public IList<T> Get(params object[] parameters)
        {
            return Get(_selectStoredProcedure, null, parameters);
        }

        protected virtual IList<T> Get(string storedProcedure, params object[] parameters)
        {
            return GetGeneric<T>(storedProcedure, null, parameters);
        }

        protected virtual IList<T> Get(string storedProcedure, DbTransaction transaction, params object[] parameters)
        {
            return GetGeneric<T>(storedProcedure, transaction, parameters);
        }

        protected virtual IList<TG> GetGeneric<TG>(string storedProcedure, params object[] parameters) where TG : class, new()
        {
            return GetGeneric<TG>(storedProcedure, null, parameters);
        }

        protected virtual IList<TG> GetGeneric<TG>(string storedProcedure, DbTransaction transaction, params object[] parameters) where TG : class, new()
        {
            var entities = new List<TG>();

            using (var command = Database.GetStoredProcCommand(storedProcedure))
            {
                command.CommandTimeout = CommandTimeout;
                LoadParameter(command, parameters);

                using (var dataReader = transaction == null ? Database.ExecuteReader(command) : Database.ExecuteReader(command, transaction))
                {
                    while (dataReader.Read())
                    {
                        var entity = GetEntity<TG>(dataReader);
                        if (entity != null) entities.Add(entity);
                    }
                }
            }
            return entities;
        }

        public IList<T> GetPaged(params object[] parameters)
        {
            return Get(_selectPagedStoredProcedure, parameters);
        }

        public T Single(params object[] parameters)
        {
            return Single(_singleStoredProcedure, parameters);
        }

        public T Single(DbTransaction transaction, params object[] parameters)
        {
            return Single(_singleStoredProcedure, transaction, parameters);
        }

        protected virtual T Single(string storedProcedure, params object[] parameters)
        {
            return SingleGeneric<T>(storedProcedure, null, parameters);
        }

        protected virtual T Single(string storedProcedure, DbTransaction transaction, params object[] parameters)
        {
            return SingleGeneric<T>(storedProcedure, transaction, parameters);
        }

        protected virtual TG SingleGeneric<TG>(string storedProcedure, params object[] parameters) where TG : class, new()
        {
            return SingleGeneric<TG>(storedProcedure, null, parameters);
        }

        protected virtual TG SingleGeneric<TG>(string storedProcedure, DbTransaction transaction = null, params object[] parameters) where TG : class, new()
        {
            var entity = default(TG);

            using (var command = Database.GetStoredProcCommand(storedProcedure))
            {
                command.CommandTimeout = CommandTimeout;
                LoadParameter(command, parameters);

                using (var dataReader = transaction == null ? Database.ExecuteReader(command) : Database.ExecuteReader(command, transaction))
                {
                    if (dataReader.Read())
                    {
                        entity = GetEntity<TG>(dataReader);
                    }
                }
            }
            return entity;
        }

        public int Count(params object[] parameters)
        {
            return Count(_countStoredProcedure, parameters);
        }

        protected virtual int Count(string storedProcedure, params object[] parameters)
        {
            var count = 0;
            using (var command = Database.GetStoredProcCommand(storedProcedure))
            {
                command.CommandTimeout = CommandTimeout;
                LoadParameter(command, parameters);

                using (var dr = Database.ExecuteReader(command))
                {
                    if (dr.Read())
                    {
                        count = dr.GetInt32(0);
                    }
                }
            }
            return count;
        }

        public T Add(T entity)
        {
            return Add(_insertStoredProcedure, entity);
        }

        protected virtual T Add(string storedProcedure, T entity, DbTransaction transaction = null)
        {
            return AddGeneric(storedProcedure, entity, transaction);
        }

        protected TG AddGeneric<TG>(string storedProcedure, TG entity, DbTransaction transaction = null) where TG : class, new()
        {
            using (var command = Database.GetStoredProcCommand(storedProcedure))
            {
                command.CommandTimeout = CommandTimeout;
                Database.DiscoverParameters(command);

                var returnParameters = new List<string>();
                foreach (DbParameter parameter in command.Parameters)
                {
                    if (parameter.Direction != ParameterDirection.InputOutput) continue;
                    returnParameters.Add(parameter.ParameterName.Replace("@", ""));
                }

                var properties = entity.GetType().GetProperties();
                foreach (DbParameter parameterSQL in command.Parameters)
                {
                    parameterSQL.Value = DBNull.Value;
                    if (parameterSQL.Direction == ParameterDirection.Output ||
                        parameterSQL.Direction == ParameterDirection.ReturnValue) continue;

                    var parameterName = parameterSQL.ParameterName.Replace("@", "").ToUpper();
                    var propiedad = properties.FirstOrDefault(p => p.Name.ToUpper() == parameterName);
                    if (propiedad == null) continue;

                    parameterSQL.Value = propiedad.GetValue(entity, null);
                }
                
                if (transaction == null)
                {
                    Database.ExecuteNonQuery(command);
                }
                else
                {
                    Database.ExecuteNonQuery(command, transaction);
                }

                foreach (var parameter in returnParameters)
                {
                    var property = entity.GetType().GetProperties().FirstOrDefault(p => p.Name.ToUpper() == parameter.ToUpper());
                    if (property == null) continue;
                    property.SetValue(entity, Database.GetParameterValue(command, parameter), null);
                }
            }
            return entity;
        }

        public T Update(T entity)
        {
            return Update(_updateStoredProcedure, entity);
        }

        protected virtual T Update(string storedProcedure, T entity, DbTransaction transaction = null)
        {
            using (var command = Database.GetStoredProcCommand(storedProcedure))
            {
                command.CommandTimeout = CommandTimeout;
                Database.DiscoverParameters(command);

                var properties = entity.GetType().GetProperties();
                foreach (DbParameter parameterSQL in command.Parameters)
                {
                    parameterSQL.Value = DBNull.Value;

                    if (parameterSQL.Direction == ParameterDirection.Output ||
                        parameterSQL.Direction == ParameterDirection.ReturnValue) continue;

                    var parameterName = parameterSQL.ParameterName.Replace("@", "").ToUpper();
                    var propiedad = properties.FirstOrDefault(p => p.Name.ToUpper() == parameterName.ToUpper());
                    if (propiedad == null) continue;

                    parameterSQL.Value = propiedad.GetValue(entity, null);
                }

                if (transaction == null)
                {
                    Database.ExecuteNonQuery(command);
                }
                else
                {
                    Database.ExecuteNonQuery(command, transaction);
                }
            }

            return entity;
        }

        public void Delete(params object[] parameters)
        {
            Delete(_deleteStoredProcedure, parameters);
        }

        protected virtual void Delete(string storedProcedure, params object[] parameters)
        {
            using (var command = Database.GetStoredProcCommand(storedProcedure))
            {
                command.CommandTimeout = CommandTimeout;
                LoadParameter(command, parameters);

                Database.ExecuteNonQuery(command);
            }
        }

        protected virtual object GetScalar(string storedProcedure, params object[] parameters)
        {
            object scalar = null;

            using (var command = Database.GetStoredProcCommand(storedProcedure))
            {
                command.CommandTimeout = CommandTimeout;
                LoadParameter(command, parameters);

                using (var dr = Database.ExecuteReader(command))
                {
                    if (dr.Read())
                    {
                        scalar = dr.GetValue(0);
                    }
                }
            }

            return scalar;
        }

        #endregion Miembros de IRepository<T>

        #region Metodos

        private static DbType GetDbType(Type type)
        {
            var name = type.Name;
            var value = DbType.String;
            try
            {
                value = (DbType)Enum.Parse(typeof(DbType), name, true);
            }
            catch (Exception)
            {
            }
            return value;
        }

        private void LoadParameter(DbCommand command, params object[] parameters)
        {
            Database.DiscoverParameters(command);

            if (parameters.Count() == 0) return;
            var index = 0;

            foreach (DbParameter parameterSQL in command.Parameters)
            {
                parameterSQL.Value = DBNull.Value;

                if (parameterSQL.Direction == ParameterDirection.Output ||
                    parameterSQL.Direction == ParameterDirection.ReturnValue) continue;

                if (command.Parameters.Count == index) break;

                var parameter = parameters[index++];
                if (parameter == null) continue;

                parameterSQL.DbType = GetDbType(parameter.GetType());
                parameterSQL.Value = parameter;
            }
        }

        protected TQ GetEntity<TQ>(IDataReader dataReader) where TQ : class, new()
        {
            var entity = new TQ();
            var fieldCount = dataReader.FieldCount;
            var properties = entity.GetType().GetProperties();

            for (var i = 0; i < fieldCount; i++)
            {
                foreach (var property in properties)
                {
                    if (!property.CanWrite) continue;
                    if (property.Name != dataReader.GetName(i)) continue;

                    var value = dataReader.GetValue(i);
                    if (value is DBNull) continue;

                    property.SetValue(entity, value, null);
                    break;
                }
            }

            return entity;
        }

        protected IList<string> GetColumns(IDataReader dataReader)
        {
            var columns = new List<string>();
            using (var schema = dataReader.GetSchemaTable())
            {
                for (var i = 0; i < schema.Rows.Count; i++)
                    columns.Add(schema.Rows[i]["ColumnName"].ToString());
            }
            return columns;
        }

        protected TQ GetValue<TQ>(IDataReader dataReader, string columnName)
        {
            var ordinal = dataReader.GetOrdinal(columnName);

            return GetValue<TQ>(dataReader, ordinal);
        }

        protected TQ GetValue<TQ>(IDataReader dataReader, int columnIndex)
        {
            if (!dataReader.IsDBNull(columnIndex))
                return (TQ)dataReader.GetValue(columnIndex);
            return default(TQ);
        }

        #endregion Metodos
    }
}