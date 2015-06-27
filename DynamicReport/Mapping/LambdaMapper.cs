using System;
using System.Data.Entity;
using System.Linq.Expressions;
using DynamicReport.SqlEngine;

namespace DynamicReport.Mapping
{
    /// <summary>
    /// Base class for all Mappers, contains common functionality which necessary for building SQL queries from C# objects.
    /// </summary>
    public abstract class LambdaMapper
    {
        protected DbContext _context;
        private readonly string _prefix;
        private EFMappingExtractor EfMappingExtractor { get; set; }

        //ToDo: Remove OuterExpressions??
        public virtual LambdaExpression[] OuterExpressions { get; set; }

        protected LambdaMapper(EFMappingExtractor efMappingExtractor) : this(efMappingExtractor, Guid.NewGuid().ToString())
        {
        }

        protected LambdaMapper(EFMappingExtractor efMappingExtractor, string prefix)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            EfMappingExtractor = efMappingExtractor;
            _prefix = prefix;
        }

        /// <summary>
        /// Extract column and table name from expression.
        /// </summary>
        /// <param name="exp">Lambda expression which represent some field. Example: (Person p) => p.Name</param>
        /// <returns>SQL analog of expression, format: [TableName].[ColumnName] </returns>
        protected string Column(LambdaExpression exp)
        {
            //return SqlConverter.LambdaExpressionToColumnName(exp);
            return string.Format("{0}.{1}", GetTableAlias(exp), EfMappingExtractor.GetSQLColumnName(exp));
        }

        /// <summary>
        /// Extract table name from C# Type EF Mapping.
        /// </summary>
        /// <param name="t">Object type</param>
        /// <returns>Name of SQL table which mapped to C# Type</returns>
        protected string Table(Type t)
        {
            //return SqlConverter.TypeToTableName(t);
            return string.Format("{0} {1}", EfMappingExtractor.GetSQLTableName(t), GetTableAlias(t));
        }

        /// <summary>
        /// Cast Expression<Func<TSource, TProperty>> to LambdaExpression
        /// </summary>
        /// <param name="exp">Expression</param>
        /// <returns>Lambda Expression</returns>
        protected LambdaExpression Lambda<TSource, TProperty>(Expression<Func<TSource, TProperty>> exp)
        {
            return exp;
        }

        private string GetTableAlias(LambdaExpression propertyLambda)
        {
            var t = EFMappingExtractor.GetTheRootObjectType(propertyLambda);

            return GetTableAlias(t);
        }

        private string GetTableAlias(Type t)
        {
            //Default alias.
            return _prefix + "_" + t.Name;
        }
    }
}
