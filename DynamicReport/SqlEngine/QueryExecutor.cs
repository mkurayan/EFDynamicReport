using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DynamicReport.SqlEngine
{
    public class QueryExecutor
    {
        private SqlConnection _connection;
        private string _connectionString = "";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="asConnectionString">Connection string.</param>
        public QueryExecutor(string asConnectionString)
        {
            _connectionString = asConnectionString;
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

        public DataTable ExecuteToDataTable(string asSql, IDataParameter[] aoaParameters, IEnumerable<string> columns)
        {
            SqlConnection loConnection = new SqlConnection(ConnectionString);
            loConnection.Open();

            try
            {
                DataTable loTable = new DataTable();

                foreach (var column in columns)
                {
                    loTable.Columns.Add(column, typeof (string));
                }

                using (SqlDataAdapter loAdapter = new SqlDataAdapter(asSql, loConnection))
                {
                    loAdapter.SelectCommand.Parameters.AddRange(aoaParameters);
                    loAdapter.Fill(loTable);
                }

                return loTable;
            }
            catch (Exception loExc)
            {
                //ToDo: Implement Logging.
                //string msg = GetSQLErrorString(asSql, aoaParameters);
                //ErrorLogger.WriteErrorLog(new Exception(msg, loExc), _connectionString);
                throw;
            }
            finally
            {
                loConnection.Dispose();      
            }
        }

        /// <summary>
        /// Builds SQL error string that contains SQL statement and SQL parameters.
        /// </summary>
        /// <param name="asSql">SQL statement.</param>
        /// <param name="aoaParameters">SQL parameters.</param>
        /// <returns>String that contains SQL statement and SQL parameters.</returns>
        public string GetSQLErrorString(string asSql, object[] aoaParameters)
        {
            if (aoaParameters == null)
                aoaParameters = new object[0];

            string lsSQLTemplate = "SQL Command: [{0}]. ";
            string lsParamsTemplate = "Parameters: [{0}].";
            StringBuilder loParams = new StringBuilder();
            foreach (object loParam in aoaParameters)
            {
                loParams.Append("{");
                loParams.Append(loParam);
                loParams.Append("}");
                loParams.Append("|");
            }

            lsSQLTemplate = String.Format(lsSQLTemplate, asSql);

            if (loParams.Length > 0)
            {
                loParams.Remove(loParams.Length - 1, 1);
                lsParamsTemplate = String.Format(lsParamsTemplate, loParams.ToString());
                lsSQLTemplate += lsParamsTemplate;
            }

            return lsSQLTemplate;
        }
    }
}