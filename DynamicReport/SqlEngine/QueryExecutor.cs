using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DynamicReport.SqlEngine
{
    public class QueryExecutor
    {
        private string _connectionString = "";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        public QueryExecutor(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Connection string.
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public DataTable ExecuteToDataTable(Query query)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                DataTable table = new DataTable();

                using (SqlDataAdapter adapter = new SqlDataAdapter(query.SqlQuery, connection))
                {
                    adapter.SelectCommand.Parameters.AddRange(query.Parameters.ToArray());
                    adapter.Fill(table);
                }

                return table;
            }
            catch (Exception e)
            {
                string msg = GetSQLErrorString(query.SqlQuery, query.Parameters);
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
        public string GetSQLErrorString(string sql, IEnumerable<IDataParameter> parameters)
        {
            if (parameters == null)
                parameters = Enumerable.Empty<IDataParameter>();

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