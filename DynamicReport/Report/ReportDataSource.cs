using System;

namespace DynamicReport.Report
{
    public class ReportDataSource : IReportDataSource
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
