using System;

namespace DynamicReport.Report
{
    /// <summary>
    /// Data source for report.
    /// </summary>
    public class ReportDataSource
    {
        public string SqlQuery { get; private set; }

        public ReportDataSource(string sqlQuery)
        {
            if (string.IsNullOrEmpty(sqlQuery))
            {
                throw new ArgumentNullException("sqlQuery");
            }

            SqlQuery = sqlQuery;
        }
    }
}
