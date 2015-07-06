using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DynamicReport.Report;
using DynamicReport.SqlEngine;

namespace DynamicReport.Mapping
{
    //ToDo: remove LambdaMapper.
    public class TableMapper<TSource> : LambdaMapper
    {
        private const string Self = "{0}";

        public TableMapper(EFMappingExtractor efMappingExtractor) : base(efMappingExtractor)
        {
        }

        public TableMapper(EFMappingExtractor efMappingExtractor, string prefix) : base(efMappingExtractor, prefix)
        {
        }

        public ReportField Field<TProperty>(string title, Expression<Func<TSource, TProperty>> property)
        {
            LambdaExpression[] lambdaExpressions = { property };
            return GetReportField(title, Self, lambdaExpressions);
        }

        public ReportField Field<TProperty>(string title, string sqlTemplate, params Expression<Func<TSource, TProperty>>[] properties)
        {
            return GetReportField(title, sqlTemplate, properties);
        }

        //ToDo: Check, do I need this method or not.
        //public ReportField Field(string title, string sqlTemplate, params LambdaExpression[] properties)
        //{
        //    return GetReportField(title, sqlTemplate, properties);
        //}

        public ReportDataSource GetReportDataSource()
        {
            string sql = Table(typeof (TSource));
            return new ReportDataSource(sql);
        }

        private ReportField GetReportField(string title, string sqlTemplate, params LambdaExpression[] outerExpressions)
        {
            var sqlValueExpression = string.Format(sqlTemplate, outerExpressions.Select(Column).ToArray());

            return new ReportField
            {
                Title = title,
                SqlValueExpression = string.Format("({0})", sqlValueExpression)
            };
        }
    }
}
