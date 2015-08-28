using System.Collections.Generic;

namespace DynamicReport.Report
{
    /// <summary>
    /// Represent dynamic report which based on SQL query.
    /// </summary>
    public interface IReportModel
    {
        /// <summary>
        /// Collection of columns which will be available for this report.
        /// </summary>
        IEnumerable<IReportColumn> ReportColumns { get; }

        /// <summary>
        /// Add report column to ReportColumns collection.
        /// </summary>
        /// <param name="reportField"></param>
        void AddReportColumn(IReportColumn reportField);

        /// <summary>
        /// Get report column from ReportColumns collection.
        /// </summary>
        /// <param name="title">Title of report column.</param>
        /// <returns>IReportField or null if there is no column with such Title.</returns>
        IReportColumn GetReportColumn(string title);

        /// <summary>
        /// Set report data source.
        /// </summary>
        /// <param name="dataSource"></param>
        void SetDataSource(IReportDataSource dataSource);


        /// <summary>
        /// Process report with proposed columns and filters, return report data.
        /// </summary>
        /// <param name="columns">Columns which will be included in report.</param>
        /// <param name="filters">Filters which will be applied on report.</param>
        /// <returns></returns>
        List<Dictionary<string, object>> Get(IEnumerable<IReportColumn> columns, IEnumerable<IReportFilter> filters);
    }
}
