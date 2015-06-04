using System;
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

        public static IDataParameter GenerateDBParameter(string parameterName, object parameterValue, SqlDbType parameterType)
        {
            if (string.IsNullOrEmpty(parameterName) || parameterValue == null)
                throw new ArgumentException();

            var parameter = new SqlParameter("@" + parameterName, parameterType)
            {
                Value = parameterValue
            };

            return parameter;
        }

        public DataTable ExecuteToDataTable(Query query)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                DataTable table = new DataTable();

                foreach (var column in query.Columns)
                {
                    table.Columns.Add(column, typeof (string));
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(query.SqlQuery, connection))
                {
                    adapter.SelectCommand.Parameters.AddRange(query.Parameters.ToArray());
                    adapter.Fill(table);
                }

                return table;
            }
            catch (Exception e)
            {
                //ToDo: Throw Repprt Exception!.
                //string msg = GetSQLErrorString(asSql, aoaParameters);
                //ErrorLogger.WriteErrorLog(new Exception(msg, loExc), _connectionString);
                throw;
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
        public string GetSQLErrorString(string sql, object[] parameters)
        {
            if (parameters == null)
                parameters = new object[0];

            string sqlTemplate = "SQL Command: [{0}]. ";
            string paramsTemplate = "Parameters: [{0}].";
            StringBuilder sqlParams = new StringBuilder();
            foreach (object loParam in parameters)
            {
                sqlParams.Append("{");
                sqlParams.Append(loParam);
                sqlParams.Append("}");
                sqlParams.Append("|");
            }

            sqlTemplate = String.Format(sqlTemplate, sql);

            if (sqlParams.Length > 0)
            {
                sqlParams.Remove(sqlParams.Length - 1, 1);
                paramsTemplate = String.Format(paramsTemplate, sqlParams.ToString());
                sqlTemplate += paramsTemplate;
            }

            return sqlTemplate;
        }
    }
}