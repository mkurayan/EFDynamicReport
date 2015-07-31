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
        private static Dictionary<FilterType, Func<string, string>> _tBag;
        private static Dictionary<FilterType, Func<string, string>> TBag 
        {
            get
            {
                if (_tBag == null)
                {
                    Func<string, string> sqlLike = x => "'%' + " + x + " + '%'";

                    _tBag = new Dictionary<FilterType, Func<string, string>>()
                    {
                        { FilterType.Include, sqlLike},
                        { FilterType.NotInclude, sqlLike}
                    };
                }

                return _tBag;
            }
        }

        public SqlCommand BuildQuery(IEnumerable<IReportField> fields, IEnumerable<IReportFilter> filters, string dataSource)
        {
            if (fields == null || !fields.Any())
            {
                throw new ReportException("Fields collection is null or empty. Impossible to build query without any output values.");
            }

            if (string.IsNullOrWhiteSpace(dataSource))
            {
                throw new ReportException("Report data source is null or empty. Impossible to build query without data source");
            }

            //Always order report columns and filters. As result SQL will not generate different compiled plans when columns in reports have different order.
            IReportField[] reportFields = fields.OrderBy(x => x.Title).ToArray();

            string colsOrder = "";

            filters = filters != null ? filters.OrderBy(x => x.ReportField.Title).ThenBy(x => x.Type).ToArray() : Enumerable.Empty<IReportFilter>();

            foreach (var fieldDefenition in reportFields)
            {
                if (IsSqlCommaNeeded(colsOrder))
                    colsOrder += ", ";

                colsOrder += string.Format("({0}) AS {1}", fieldDefenition.SqlValueExpression, fieldDefenition.SqlAlias);
            }

            var sqlParams = new List<IDataParameter>();
            string sqlFilter = "";
            foreach (var filter in filters)
            {
                var formattedFilterValue = filter.ReportField.InputValueTransformation != null
                    ? filter.ReportField.InputValueTransformation(filter.Value)
                    : filter.Value;

                var parameter = GenerateDBParameter("p" + sqlParams.Count, formattedFilterValue, SqlDbType.NVarChar);
                sqlParams.Add(parameter);

                if (!string.IsNullOrEmpty(sqlFilter))
                    sqlFilter += " AND ";

                sqlFilter += BuildSqlFilter(filter.Type, filter.ReportField.SqlValueExpression, parameter.ParameterName);
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

        private static string BuildSqlFilter(FilterType filterType, string sqlValueExpression, string sqlpParameterName)
        {
            //Apply SQL transformation. Wrap walue to %..% symbols and so on.
            if (TBag.ContainsKey(filterType))
            {
                sqlpParameterName = TBag[filterType](sqlpParameterName);
            }
            

            //Wrap SQL query with 'isnull(...) check' in ordet to correct checking for inequality.
            //For more detail see http://stackoverflow.com/questions/5618357/sql-server-null-vs-empty-string
            sqlValueExpression = "isnull(" + sqlValueExpression + ",'')";

            string sqlOperator;
            switch (filterType)
            {
                case FilterType.Equal:
                    sqlOperator = " = ";
                    break;
                case FilterType.NotEqual:
                    sqlOperator = " != ";
                    break;
                case FilterType.GreatThenOrEqualTo:
                    sqlOperator = " >= ";
                    break;
                case FilterType.GreatThen:
                    sqlOperator = " > ";
                    break;
                case FilterType.LessThenOrEquaslTo:
                    sqlOperator = " <= ";
                    break;
                case FilterType.LessThen:
                    sqlOperator = " < ";
                    break;
                case FilterType.Include:
                    sqlOperator = " like ";
                    break;
                case FilterType.NotInclude:
                    sqlOperator = " not like ";
                    break;
                default:
                    sqlOperator = " = ";
                    break;
            }

            return sqlValueExpression + sqlOperator + sqlpParameterName;
        }

        private static IDataParameter GenerateDBParameter(string parameterName, object parameterValue, SqlDbType parameterType)
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