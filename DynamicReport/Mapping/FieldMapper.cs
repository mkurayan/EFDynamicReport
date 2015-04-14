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
    /// This class represent single field mapping. 
    /// Lambda expression will be converted to SQL expression which will be used inside Report.
    /// </summary>
    public class FieldMapper
    {
        private SqlConverter _sqlConverter;

        public string Title { get; set; }

        public virtual LambdaExpression[] OuterExpressions { get; set; }

        public virtual string SqlTemplate { get; set; }

        protected virtual Func<string, string> OutputValueTransformation
        {
            get { return x => x; }
        }

        protected virtual Func<string, string> InputValueTransformation
        {
            get { return x => x; }
        }

        public FieldMapper(DbContext context)
        {
            _sqlConverter = new SqlConverter(new EFMappingExtractor(context));
        }

        public ReportField ConverLambdaExpressionToSql(SqlConverter queryBuilder)
        {
            var sqlValueExpression = string.Format(SqlTemplate, OuterExpressions.Select(x => queryBuilder.LambdaExpressionToColumnName(x)));

            return new ReportField()
            {
                Title = this.Title,
                SqlValueExpression = string.Format("({0})", sqlValueExpression),
                InputValueTransformation = this.InputValueTransformation,
                OutputValueTransformation = this.OutputValueTransformation
            };
        }

        protected string Column<TSource, TProperty>(Expression<Func<TSource, TProperty>> exp, string tableAlias)
        {
            return _sqlConverter.LambdaExpressionToColumnName(exp, tableAlias);
        }

        protected string Column<TSource, TProperty>(Expression<Func<TSource, TProperty>> exp)
        {
            return _sqlConverter.LambdaExpressionToColumnName(exp, null);
        }

        protected string Table(Type t, string tableAlias)
        {
            return _sqlConverter.TypeToTableName(t, tableAlias);
        }

        protected string Table(Type t)
        {
            return _sqlConverter.TypeToTableName(t, null);
        }

        protected LambdaExpression Lambda<TSource, TProperty>(Expression<Func<TSource, TProperty>> exp)
        {
            return exp;
        }
    }
}
