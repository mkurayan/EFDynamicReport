namespace DynamicReport.Report
{
    public class ReportFilter
    {
        public string ReportFieldTitle { get; set; }
        public ReportFilterType FilterType { get; set; }
        public string Value { get; set; }
        
        //ToDo: instead this format walue directly during query building.
        public string FormattedValue { get; set; }
    }
}