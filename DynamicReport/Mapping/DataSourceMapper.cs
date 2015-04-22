using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicReport.Report;
using DynamicReport.SqlEngine;

namespace DynamicReport.Mapping
{
    public class DataSourceMapper : EFMapper
    {
        public DataSourceMapper(SqlConverter converter)
            : base(converter)
        {
        }

        public ReportDataSource ConverLambdaExpressionToSql(SqlConverter outerConverter)
        {
            var sqlValueExpression = string.Format(SqlTemplate, OuterExpressions.Select(outerConverter.LambdaExpressionToColumnName));

            return new ReportDataSource(sqlValueExpression);
        }
    }
}
