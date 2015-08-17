using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DynamicReport.SqlEngine
{
    public class QueryExecutor : IQueryExecutor
    {
        private string _connectionString = "";

        /// <summary>
        /// Create Query executor.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        public QueryExecutor(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(connectionString);
            }

            _connectionString = connectionString;
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public DataTable ExecuteToDataTable(SqlCommand selectCommand)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                //use connection to DB.
                selectCommand.Connection = connection;

                DataTable table = new DataTable();
                
                using (SqlDataAdapter adapter = new SqlDataAdapter(selectCommand))
                {
                    adapter.Fill(table);
                }

                return table;
            }
            catch (Exception)
            {
                string msg = GetSQLErrorString(selectCommand.CommandText, selectCommand.Parameters);
                throw new ReportException(msg);
            }
            finally
            {
                connection.Dispose();      
            }
        }

        /// <summary>
        /// Builds SQL error string that contains SQL statement and SQL parameters.
        /// </summary>
        /// <param name="sql">SQL statement.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <returns>String that contains SQL statement and SQL parameters.</returns>
        private string GetSQLErrorString(string sql, SqlParameterCollection parameters)
        {
            string sqlTemplate = "SQL Command: [{0}]. ";
            string paramsTemplate = "Parameters: [{0}].";
            StringBuilder sqlParams = new StringBuilder();
            foreach (var parameter in parameters)
            {
                sqlParams.Append("{");
                sqlParams.Append(parameter);
                sqlParams.Append("}");
                sqlParams.Append("|");
            }

            sqlTemplate = String.Format(sqlTemplate, sql);

            if (sqlParams.Length > 0)
            {
                sqlParams.Remove(sqlParams.Length - 1, 1);
                paramsTemplate = String.Format(paramsTemplate, sqlParams);
                sqlTemplate += paramsTemplate;
            }

            return sqlTemplate;
        }
    }
}