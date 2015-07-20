namespace DynamicReport.Report
{
    /// <summary>
    /// Represent custom filter which could be applied against report data.
    /// Result report will contains only data which satisfy to filters.
    /// </summary>
    public interface IReportFilter
    {
        string ReportFieldTitle { get; set; }
        FilterType Type { get; set; }
        string Value { get; set; }
    }
}
