using System;
using System.Linq;
using System.Linq.Expressions;
using DynamicReport.Report;
using DynamicReport.SqlEngine;

namespace DynamicReport.MappingHelpers
{
    public class TableMapper<TSource>
    {
        private const string Self = "{0}";

        /// <summary>
        /// This prefix is used for the creation of SQL Aliases.
        /// </summary>
        public string TableAliasPrefix { get; private set; }

        protected IQueryExtractor QueryExtractor { get; set; }

        public TableMapper(IQueryExtractor queryExtractor)
            : this(queryExtractor, GeneratePrefix())
        {
        }

        public TableMapper(IQueryExtractor queryExtractor, string tableAliasPrefix)
        {
            if (tableAliasPrefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            QueryExtractor = queryExtractor;
            TableAliasPrefix = tableAliasPrefix;
        }

        /// <summary>
        /// Create ReportColumn from Lambda mapping.
        /// </summary>
        /// <typeparam name="TProperty">C# type of output column.</typeparam>
        /// <param name="title">Title for column, this title will be used in report.</param>
        /// <param name="property">Expression which represent output column.</param>
        /// <returns>ReportColumn object</returns>
        public IReportColumn Column<TProperty>(string title, Expression<Func<TSource, TProperty>> property)
        {
            return GetReportColumn(title, Self, property);
        }

        /// <summary>
        /// Create ReportColumn from Lambda mapping.
        /// </summary>
        /// <typeparam name="TProperty">C# type of output column.</typeparam>
        /// <param name="title">Title for column, this title will be used in report.</param>
        /// <param name="properties">Expressions which represents output column.</param>
        /// <param name="sqlTemplate"> Allow to set custom format for output column or provide other transformations like combining or subtraction of output columns.</param>
        /// <returns>ReportColumn object</returns>
        public IReportColumn Column<TProperty>(string title, string sqlTemplate, params Expression<Func<TSource, TProperty>>[] properties)
        {
            return GetReportColumn(title, sqlTemplate, properties);
        }

        public IReportDataSource GetReportDataSource()
        {
            string sql = Table();
            return new ReportDataSource(sql);
        }

        /// <summary>
        /// Extract column and table name from expression.
        /// </summary>
        /// <param name="exp">Lambda expression which represent some column. Example: (Person p) => p.Name</param>
        /// <returns>SQL analog of expression, format: [TableName].[ColumnName] </returns>
        public string Column<TProperty>(Expression<Func<TSource, TProperty>> exp)
        {
            return string.Format("{0}.{1}", GetTableAlias(typeof(TSource)), QueryExtractor.GetSQLColumnName(exp));
        }

        /// <summary>
        /// Extract table name from C# Type EF Mapping.
        /// </summary>
        /// <param name="t">Object type</param>
        /// <returns>Name of SQL table which mapped to C# Type</returns>
        public string Table()
        {
            return string.Format("{0} {1}", QueryExtractor.GetSQLTableName(typeof(TSource)), GetTableAlias(typeof(TSource)));
        }

        private IReportColumn GetReportColumn<TProperty>(string title, string sqlTemplate, params Expression<Func<TSource, TProperty>>[] outerExpressions)
        {
            var sqlValueExpression = string.Format(sqlTemplate, outerExpressions.Select(Column).ToArray());

            return new ReportColumn
            {
                Title = title,
                SqlValueExpression = string.Format("({0})", sqlValueExpression)
            };
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
