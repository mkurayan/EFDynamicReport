using System;
using System.Linq;
using DynamicReport.Report;
using DynamicReport.SqlEngine;

namespace DynamicReport.Mapping
{
    /// <summary>
    /// This class represent single field mapping. 
    /// Lambda expression will be converted to SQL expression which will be used inside Report.
    /// </summary>
    public class FieldMapper : LambdaMapper
    {
        public string SqlTemplate { get; set; }

        public string Title { get; set; }

        protected virtual Func<string, string> OutputValueTransformation
        {
            get { return x => x; }
        }

        protected virtual Func<string, string> InputValueTransformation
        {
            get { return x => x; }
        }

        public FieldMapper(EFMappingExtractor efMappingExtractor, string prefix)
            : base(efMappingExtractor, prefix)
        {
        }

        public ReportField GetReportField()
        {
            var sqlValueExpression = string.Format(SqlTemplate, OuterExpressions.Select(Column).ToArray());

            return new ReportField
            {
                Title = this.Title,
                SqlValueExpression = string.Format("({0})", sqlValueExpression),
                InputValueTransformation = this.InputValueTransformation,
                OutputValueTransformation = this.OutputValueTransformation
            };
        }
    }
}
