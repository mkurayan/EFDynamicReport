﻿using System.Collections.Generic;

namespace DynamicReport.Report
{
    /// <summary>
    /// Represent dynamic report which based on SQL query.
    /// </summary>
    public interface IReportModel
    {
        /// <summary>
        /// Collection of fields which will be available for this report.
        /// </summary>
        IEnumerable<IReportField> ReportFields { get; }

        /// <summary>
        /// Add report field to ReportFields collection.
        /// </summary>
        /// <param name="reportField"></param>
        void AddReportField(IReportField reportField);

        /// <summary>
        /// Get report field from ReportFields collection.
        /// </summary>
        /// <param name="title">Title of report field.</param>
        /// <returns>IReportField or null if there is no field with such Title.</returns>
        IReportField GetReportField(string title);

        /// <summary>
        /// Set report data source.
        /// </summary>
        /// <param name="dataSource"></param>
        void SetDataSource(IReportDataSource dataSource);


        /// <summary>
        /// Process report with proposed fields and filters, return report data.
        /// </summary>
        /// <param name="columns">Fields which will be included in report.</param>
        /// <param name="filters">Filters which will be applied on report.</param>
        /// <returns></returns>
        List<Dictionary<string, object>> Get(IEnumerable<IReportField> columns, IEnumerable<IReportFilter> filters);
    }
}
