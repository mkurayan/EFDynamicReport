namespace DynamicReport.Report
{
    public class ReportFilter : IReportFilter
    {
        public IReportField ReportField { get; set; }

        public FilterType Type { get; set; }

        public string Value { get; set; }
    }
}