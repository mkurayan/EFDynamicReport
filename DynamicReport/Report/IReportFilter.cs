namespace DynamicReport.Report
{
    /// <summary>
    /// Represent custom filter which could be applied against report data.
    /// Result report will contains only data which satisfy to the filters.
    /// </summary>
    public interface IReportFilter
    {
        /// <summary>
        /// Target report field.
        /// </summary>
        IReportField ReportField { get; set; }

        /// <summary>
        /// Type of the filter.
        /// </summary>
        FilterType Type { get; set; }

        /// <summary>
        /// Filter valuel.
        /// </summary>
        string Value { get; set; }
    }

    /// <summary>
    /// This enum represent the set of comparison operators which available in report system.
    /// </summary>
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
