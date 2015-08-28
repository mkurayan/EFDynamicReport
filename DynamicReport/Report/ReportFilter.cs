namespace DynamicReport.Report
{
    public class ReportFilter : IReportFilter
    {
        public IReportColumn ReportColumn { get; set; }

        public FilterType Type { get; set; }

        public string Value { get; set; }
    }
}