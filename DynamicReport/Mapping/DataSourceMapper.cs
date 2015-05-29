using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
            string[] tableNames = OuterExpressions
                .Select(outerExpression => outerConverter.TypeToTableName(((ParameterExpression) outerExpression.Body).Type))
                .ToArray();

            return new ReportDataSource(string.Format(SqlTemplate, tableNames));
        }
    }
}
