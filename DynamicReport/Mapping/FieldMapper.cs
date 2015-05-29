using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DynamicReport.Report;
using DynamicReport.SqlEngine;

namespace DynamicReport.Mapping
{
    /// <summary>
    /// This class represent single field mapping. 
    /// Lambda expression will be converted to SQL expression which will be used inside Report.
    /// </summary>
    public class FieldMapper : EFMapper
    {
        public string Title { get; set; }

        protected virtual Func<string, string> OutputValueTransformation
        {
            get { return x => x; }
        }

        protected virtual Func<string, string> InputValueTransformation
        {
            get { return x => x; }
        }

        public FieldMapper(SqlConverter context)
            : base(context)
        {
        }

        public ReportField ConverLambdaExpressionToSql(SqlConverter outerConverter)
        {
            var sqlValueExpression = string.Format(SqlTemplate, OuterExpressions.Select(outerConverter.LambdaExpressionToColumnName).ToArray());

            return new ReportField()
            {
                Title = this.Title,
                SqlValueExpression = string.Format("({0})", sqlValueExpression),
                InputValueTransformation = this.InputValueTransformation,
                OutputValueTransformation = this.OutputValueTransformation
            };
        }
    }
}
