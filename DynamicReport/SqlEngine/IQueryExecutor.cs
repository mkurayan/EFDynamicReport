using System.Data;
using System.Data.SqlClient;

namespace DynamicReport.SqlEngine
{
    public interface IQueryExecutor
    {
        /// <summary>
        /// Connection string.
        /// </summary>
        string ConnectionString { get; }

        DataTable ExecuteToDataTable(SqlCommand query);
    }
}
