using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DynamicReport.Report;
using DynamicReport.SqlEngine;

namespace DynamicReport.Mapping
{
    /// <summary>
    /// Wrapper around report model which provide easy way for report configuration, contain a collection of helping methods.
    /// </summary>
    public abstract class ReportMapper
    {
        private const string Self = "{0}";
        protected DbContext _context;
        protected SqlConverter _sqlConverter;

        protected abstract IEnumerable<FieldMapper> ReportFields { get; }

        /// <summary>
        /// Data source for report.
        /// </summary>
        protected abstract DataSourceMapper DataSource { get; }

        protected ReportMapper(DbContext context)
        {
            _context = context;
            _sqlConverter = new SqlConverter(new EFMappingExtractor(_context));
        }

        /// <summary>
        /// Bild report model from Lambda mapping.
        /// </summary>
        /// <returns></returns>
        public ReportModel BuildReportModel()
        {
            var convertor = new SqlConverter(new EFMappingExtractor(_context), "root_ds");

            var model = new ReportModel(
                new QueryBuilder(),
                new QueryExecutor(_context.Database.Connection.ConnectionString));

            model.SetDataSource(DataSource.ConverLambdaExpressionToSql(convertor));

            foreach (var repotField in ReportFields.Select(x => x.ConverLambdaExpressionToSql(convertor)))
            {
                model.AddReportField(repotField);
            }

            return model;
        }

        protected FieldMapper FieldFromLambda<TSource, TProperty>(string title, Expression<Func<TSource, TProperty>> property)
        {
            LambdaExpression[] lambdaExpressions = { property };
            return new FieldMapper(new SqlConverter(new EFMappingExtractor(_context))) { Title = title, SqlTemplate = Self, OuterExpressions = lambdaExpressions };
        }

        protected FieldMapper FieldFromTemplate<TSource, TProperty>(string title, string sqlTemplate, params Expression<Func<TSource, TProperty>>[] properties)
        {
            return new FieldMapper(new SqlConverter(new EFMappingExtractor(_context))) { Title = title, SqlTemplate = sqlTemplate, OuterExpressions = properties };
        }

        protected FieldMapper FieldFromTemplate(string title, string sqlTemplate, params LambdaExpression[] properties)
        {
            return new FieldMapper(new SqlConverter(new EFMappingExtractor(_context))) { Title = title, SqlTemplate = sqlTemplate, OuterExpressions = properties };
        }

        protected LambdaExpression Lambda<TSource, TProperty>(Expression<Func<TSource, TProperty>> exp)
        {
            return exp;
        }

        protected DataSourceMapper FromLambda<TSource, TProperty>(Expression<Func<TSource, TProperty>> property)
        {
            LambdaExpression[] lambdaExpressions = { property };
            return new DataSourceMapper(new SqlConverter(new EFMappingExtractor(_context))) { SqlTemplate = Self, OuterExpressions = lambdaExpressions };
        }

        protected DataSourceMapper FromTemplate(string sqlTemplate, params LambdaExpression[] properties)
        {
            return new DataSourceMapper(_sqlConverter) { SqlTemplate = Self, OuterExpressions = properties };
        }
    }
}
