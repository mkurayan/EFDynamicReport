using System;

namespace DynamicReport.Report
{
    /// <summary>
    /// Represent single column in Report.
    /// </summary>
    public interface IReportColumn
    {
        /// <summary>
        /// Column title in report.
        /// Example: "Full Name"
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Part of SQL query, describes report column data source.
        /// </summary>
        string SqlValueExpression { get; }

        /// <summary>
        /// Sql alias which will be used for current column. 
        /// Example: Select (p.FirstName + p.LastName) as fullName From ...
        /// </summary>
        string SqlAlias { get; }

        /// <summary>
        /// This transformation will be applied on current column value before it will be passed to client.
        /// Allow to apply any custom transformation, examples: 
        /// 1. Convert data from DB format to customer format
        /// 2. Apply any calculation
        /// 3. Obfuscate or Encrypt sensitive data
        /// 4. Other usages...
        /// </summary>
        Func<string, string> OutputValueTransformation { get; }

        /// <summary>
        /// This transformation will be applied on user input before it will be used as filter value.
        /// Allow to apply any custom transformation, examples: 
        /// 1. Convert data from customer format to DB format
        /// 2. Apply any calculation
        /// 3. Decrypt sensitive data
        /// 4. Other usages...
        /// </summary>
        Func<string, string> InputValueTransformation { get; }


        /// <summary>
        /// This Fucn allow to customize search filter logic.
        /// This transformation modify search condition which will be generated for this column when customer apply filters against it.
        /// Parameters:
        /// 1. filter value
        /// 2. filter type
        /// 3. SQL parameter name 
        /// </summary>
        Func<string, FilterType, string, string> SearchConditionTransformation { get; }
    }
}
