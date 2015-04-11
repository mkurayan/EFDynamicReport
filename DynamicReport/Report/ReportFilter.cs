namespace DynamicReport.Report
{
    public class ReportFilter
    {
        public string ReportFieldTitle { get; set; }
        public ReportFilterType FilterType { get; set; }
        public string Value { get; set; }
        public string FormattedValue { get; set; }
    }
}