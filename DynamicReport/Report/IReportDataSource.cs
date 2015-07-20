namespace DynamicReport.Report
{
    /// <summary>
    /// Represent report data source. Usually single table or several joined tables.
    /// </summary>
    public interface IReportDataSource
    {
        string SqlQuery { get; }
    }
}
