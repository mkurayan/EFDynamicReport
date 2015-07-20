using System.Collections.Generic;

namespace DynamicReport.Report
{
    public interface IReportModel
    {
        IEnumerable<IReportField> ReportFields { get; }

        void AddReportField(IReportField reportField);

        void SetDataSource(IReportDataSource dataSource);

        List<Dictionary<string, object>> Get(IEnumerable<string> columns, IEnumerable<IReportFilter> filters);
    }
}
