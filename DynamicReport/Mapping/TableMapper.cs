using System;
using System.Linq;
using System.Linq.Expressions;
using DynamicReport.Report;
using DynamicReport.SqlEngine;

namespace DynamicReport.Mapping
{
    public class TableMapper<TSource>
    {
        private const string Self = "{0}";

        /// <summary>
        /// This prefix is used for the creation of SQL Aliases.
        /// </summary>
        public string TableAliasPrefix { get; private set; }

        protected EFMappingExtractor EfMappingExtractor { get; set; }

        public TableMapper(EFMappingExtractor efMappingExtractor)
            : this(efMappingExtractor, GeneratePrefix())
        {
        }

        public TableMapper(EFMappingExtractor efMappingExtractor, string tableAliasPrefix)
        {
            if (tableAliasPrefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            EfMappingExtractor = efMappingExtractor;
            TableAliasPrefix = tableAliasPrefix;
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
            string sql = Table();
            return new ReportDataSource(sql);
        }

        /// <summary>
        /// Extract column and table name from expression.
        /// </summary>
        /// <param name="exp">Lambda expression which represent some field. Example: (Person p) => p.Name</param>
        /// <returns>SQL analog of expression, format: [TableName].[ColumnName] </returns>
        public string Column<TProperty>(Expression<Func<TSource, TProperty>> exp)
        {
            return Column((LambdaExpression)exp);
        }

        /// <summary>
        /// Extract column and table name from expression.
        /// </summary>
        /// <param name="exp">Lambda expression which represent some field. Example: (Person p) => p.Name</param>
        /// <returns>SQL analog of expression, format: [TableName].[ColumnName] </returns>
        public string Column(LambdaExpression exp)
        {
            return string.Format("{0}.{1}", GetTableAlias(exp), EfMappingExtractor.GetSQLColumnName(exp));
        }

        /// <summary>
        /// Extract table name from C# Type EF Mapping.
        /// </summary>
        /// <param name="t">Object type</param>
        /// <returns>Name of SQL table which mapped to C# Type</returns>
        public string Table()
        {
            //return SqlConverter.TypeToTableName(t);
            return string.Format("{0} {1}", EfMappingExtractor.GetSQLTableName(typeof(TSource)), GetTableAlias(typeof(TSource)));
        }

        /// <summary>
        /// Cast Expression<Func<TSource, TProperty>> to LambdaExpression
        /// </summary>
        /// <param name="exp">Expression</param>
        /// <returns>Lambda Expression</returns>
        public LambdaExpression Lambda<TSource, TProperty>(Expression<Func<TSource, TProperty>> exp)
        {
            return exp;
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
