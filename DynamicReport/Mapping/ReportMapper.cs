using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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
        private DbContext _context;

        protected abstract IEnumerable<FieldMapper> ReportFields { get; }
        protected abstract Dictionary<string, Type> TablesAliases { get; }
        protected abstract string SqlQuery { get; }

        protected ReportMapper(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Bild report model from Lambda mapping.
        /// </summary>
        /// <returns></returns>
        public ReportModel BuildReportModel()
        {
            var convertor = new SqlConverter(new EFMappingExtractor(_context), TablesAliases);

            var reportFields = ReportFields.Select(x => x.ConverLambdaExpressionToSql(convertor));

            return new ReportModel(reportFields, SqlQuery);
        }

        protected FieldMapper FromLambda<TSource, TProperty>(string title, Expression<Func<TSource, TProperty>> property)
        {
            LambdaExpression[] lambdaExpressions = {property};
            return new FieldMapper(_context) { Title = title, SqlTemplate = Self, OuterExpressions = lambdaExpressions };
        }

        protected FieldMapper FromTemplate<TSource, TProperty>(string title, string sqlTemplate, params Expression<Func<TSource, TProperty>>[] properties)
        {
            return new FieldMapper(_context) { Title = title, SqlTemplate = Self, OuterExpressions = properties };
        }

        protected FieldMapper FromTemplate(string title, string sqlTemplate, params LambdaExpression[] properties)
        {
            return new FieldMapper(_context) { Title = title, SqlTemplate = Self, OuterExpressions = properties };
        }

        protected LambdaExpression Lambda<TSource, TProperty>(Expression<Func<TSource, TProperty>> exp)
        {
            return exp;
        }
    }
}
