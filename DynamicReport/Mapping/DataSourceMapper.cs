using System.Linq;
using System.Linq.Expressions;
using DynamicReport.Report;
using DynamicReport.SqlEngine;

namespace DynamicReport.Mapping
{
    public class DataSourceMapper : LambdaMapper
    {
        public string SqlTemplate { get; set; }

        public DataSourceMapper(EFMappingExtractor efMappingExtractor, string prefix)
            : base(efMappingExtractor, prefix)
        {
        }

        public ReportDataSource GetReportDataSource()
        {
            var sql = string.Format(SqlTemplate, OuterExpressions.Select(x => x.Body.Type).Select(Table).ToArray());

            return new ReportDataSource(sql);
        }
    }
}
