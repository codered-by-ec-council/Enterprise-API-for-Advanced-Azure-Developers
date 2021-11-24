using Polly;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECC.Customer.DataAccessLayer.SQLCommands
{
    public abstract class SQLExecuterBase<T>
    {
        protected abstract IDbConnection GetConnection();
        protected string CommandText { private get; set; }
        protected CommandType CommandType { private get; set; }
        protected IList<IDataParameter> Parameters { private get; set; }
        protected MapperReaderBase<T> Mapper { private get; set; }
        protected abstract Policy GetPolicy();


        /// <summary>
        /// Execute a SQL Datareader
        /// </summary>
        /// <returns></returns>
        internal Collection<T> ExecuteReader()
        {
            Collection<T> collection = new Collection<T>();

            using (IDbConnection connection = GetConnection())
            {
                IDbCommand command = connection.CreateCommand();
                command.Connection = connection;
                command.CommandText = this.CommandText;
                command.CommandType = this.CommandType;

                if (Parameters != null)
                {
                    foreach (IDataParameter param in this.Parameters)
                        command.Parameters.Add(param);
                }

                try
                {
                    //add the retry policy
                    GetPolicy().Execute(() =>
                    {

                        if (connection.State == ConnectionState.Closed) connection.Open();
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            try
                            {
                                collection = Mapper.MapAll(reader);

                            }
                            catch
                            {
                                throw;
                            }
                            finally
                            {
                                reader.Close();
                            }
                        }
                    });
                    return collection;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        internal int ExecuteNonQuery()
        {
            int rowsAffected = 0;
            using (IDbConnection connection = GetConnection())
            {
                IDbCommand command = connection.CreateCommand();
                command.Connection = connection;
                command.CommandText = this.CommandText;
                command.CommandType = this.CommandType;

                foreach (IDataParameter param in this.Parameters)
                {                                        
                    command.Parameters.Add(param);                   
                }
                try
                {
                    //add the retry policy
                    GetPolicy().Execute(() =>
                    {

                        if (connection.State == ConnectionState.Closed) connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    });

                }
                catch
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
            return rowsAffected;
        }

        internal Dictionary<string, object> ExecuteNonQueryWithOutputParams()
        {
            int rowsAffected = 0;
            var outVals = new Dictionary<string, object>();
            using (IDbConnection connection = GetConnection())
            {
                IDbCommand command = connection.CreateCommand();
                command.Connection = connection;
                command.CommandText = this.CommandText;
                command.CommandType = this.CommandType;
                
                if (Parameters != null)
                {
                    foreach (IDataParameter param in this.Parameters)
                        command.Parameters.Add(param);
                }

                try
                {
                    //add the retry policy
                    GetPolicy().Execute(() =>
                    {
                        if (connection.State == ConnectionState.Closed) connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                        
                        foreach (IDataParameter param in this.Parameters)
                        {                           
                            if (param.Direction == ParameterDirection.Output || param.Direction == ParameterDirection.ReturnValue)
                            {
                                outVals.Add(param.ParameterName, param.Value);
                            }
                        }
                    });
                }
                catch
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
            return outVals;
        }
    }
}
