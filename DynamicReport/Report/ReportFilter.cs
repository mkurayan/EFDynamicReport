namespace DynamicReport.Report
{
    public class ReportFilter : IReportFilter
    {
        public string ReportFieldTitle { get; set; }
        public FilterType Type { get; set; }
        public string Value { get; set; }
    }

    public enum FilterType
    {
        Unknown = 0,
        Equal = 1,
        NotEqual = 2,
        GreatThenOrEqualTo = 3,
        GreatThen = 4,
        LessThenOrEquaslTo = 5,
        LessThen = 6,
        Include = 7,
        NotInclude = 8
    }
}