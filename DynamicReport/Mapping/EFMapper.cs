using System;
using System.Data.Entity;
using System.Linq.Expressions;
using DynamicReport.SqlEngine;

namespace DynamicReport.Mapping
{
    public class EFMapper
    {
        protected DbContext _context;
        public SqlConverter SqlConverter { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string SqlTemplate { get; set; }

        public virtual LambdaExpression[] OuterExpressions { get; set; }

        protected EFMapper(SqlConverter converter)
        {
            SqlConverter = converter;
        }

        protected string Column<TSource, TProperty>(Expression<Func<TSource, TProperty>> exp)
        {
            return SqlConverter.LambdaExpressionToColumnName(exp);
        }

        protected string Column(LambdaExpression exp)
        {
            return SqlConverter.LambdaExpressionToColumnName(exp);
        }

        protected string Table(Type t)
        {
            return SqlConverter.TypeToTableName(t);
        }

        protected LambdaExpression Lambda<TSource, TProperty>(Expression<Func<TSource, TProperty>> exp)
        {
            return exp;
        }
    }
}
