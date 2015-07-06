using System;
using System.Linq.Expressions;
using DynamicReport.SqlEngine;

namespace DynamicReport.Mapping
{
    /// <summary>
    /// Base class for all Mappers, contains common functionality which necessary for building SQL queries from C# objects.
    /// </summary>
    public abstract class LambdaMapper
    {    
        /// <summary>
        /// This prefix is used for the creation of SQL Aliases.
        /// </summary>
        public string TableAliasPrefix { get; private set; }

        protected EFMappingExtractor EfMappingExtractor { get; set; }

        protected LambdaMapper(EFMappingExtractor efMappingExtractor)
            : this(efMappingExtractor, GeneratePrefix())
        {
        }

        protected LambdaMapper(EFMappingExtractor efMappingExtractor, string tableAliasPrefix)
        {
            if (tableAliasPrefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            EfMappingExtractor = efMappingExtractor;
            TableAliasPrefix = tableAliasPrefix;
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
            return t.Name + "_" + TableAliasPrefix;
        }

        /// <summary>
        /// The following method creates the shorter string and it is actually very unique. 
        /// An iteration of 10 million didn’t create a duplicate. 
        /// It uses the uniqueness of a GUID to create the string.
        /// </summary>
        /// <returns></returns>
        private static string GeneratePrefix()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
    }
}
