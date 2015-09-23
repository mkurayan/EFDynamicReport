using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DynamicReport.Report;

namespace DynamicReport.SqlEngine
{
    public class QueryBuilder : IQueryBuilder
    {
        public SqlCommand BuildQuery(IEnumerable<IReportColumn> columns, IEnumerable<IReportFilter> filters, string dataSource)
        {
            if (columns == null || !columns.Any())
            {
                throw new ReportException("Columns collection is null or empty. Impossible to build query without any output values.");
            }

            if (string.IsNullOrWhiteSpace(dataSource))
            {
                throw new ReportException("Report data source is null or empty. Impossible to build query without data source");
            }

            //Always order report columns and filters. As result SQL will not generate different compiled plans when columns in reports have different order.
            IReportColumn[] reportColumns = columns.OrderBy(x => x.Title).ToArray();
            IReportFilter[] reportFilters = 
                filters != null ? 
                filters.OrderBy(x => x.ReportColumn.Title).ThenBy(x => x.Type).ToArray() 
                : Enumerable.Empty<IReportFilter>().ToArray();

            string colsOrder = "";

            foreach (var columnDefenition in reportColumns)
            {
                if (IsSqlCommaNeeded(colsOrder))
                    colsOrder += ", ";

                colsOrder += string.Format("({0}) AS {1}", columnDefenition.SqlValueExpression, columnDefenition.SqlAlias);
            }

            var sqlParams = new List<IDataParameter>();
            string sqlFilter = "";
            foreach (var filter in reportFilters)
            {
                var parameter = GenerateDbParameter("p" + sqlParams.Count, filter.FormattedValue, SqlDbType.NVarChar);
                sqlParams.Add(parameter);

                if (!string.IsNullOrEmpty(sqlFilter))
                    sqlFilter += " AND ";

                sqlFilter += filter.BuildSqlFilter(parameter.ParameterName);
            }

            string sqlQuery = string.Format("SELECT {0} FROM {1}", colsOrder, dataSource);
            if (!string.IsNullOrEmpty(sqlFilter))
            {
                sqlQuery += string.Format(" WHERE {0}", sqlFilter);
            }

            var command = new SqlCommand(sqlQuery);
            foreach (IDataParameter dataParameter in sqlParams)
            {
                command.Parameters.Add(dataParameter);
            }

            return command;
        }

        private static bool IsSqlCommaNeeded(string sql)
        {
            return !string.IsNullOrWhiteSpace(sql) && !sql.EndsWith(",");
        }

        
        private static IDataParameter GenerateDbParameter(string parameterName, object parameterValue, SqlDbType parameterType)
        {
            if (string.IsNullOrEmpty(parameterName) || parameterValue == null)
                throw new ArgumentException();

            var parameter = new SqlParameter("@" + parameterName, parameterType)
            {
                Value = parameterValue
            };

            return parameter;
        }
    }
}