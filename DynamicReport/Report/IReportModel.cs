using System.Collections.Generic;

namespace DynamicReport.Report
{
    public interface IReportModel
    {
        IEnumerable<IReportField> ReportFields { get; }

        void AddReportField(IReportField reportField);

        IReportField GetReportField(string title);

        void SetDataSource(IReportDataSource dataSource);

        List<Dictionary<string, object>> Get(IEnumerable<IReportField> columns, IEnumerable<IReportFilter> filters);
    }
}
