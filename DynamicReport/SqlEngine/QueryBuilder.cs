using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using DynamicReport.Report;

namespace DynamicReport.SqlEngine
{
    public class QueryBuilder
    {
        private const string SqlQueryTemplate = "SELECT {0} FROM {1} WHERE {2}";

        private static Dictionary<ReportFilter.FilterType, Func<string, string>> _tBag;
        private static Dictionary<ReportFilter.FilterType, Func<string, string>> TBag 
        {
            get
            {
                if (_tBag == null)
                {
                    Func<string, string> sqlLike = x => "'%' + " + x + " + '%'";

                    _tBag = new Dictionary<ReportFilter.FilterType, Func<string, string>>()
                    {
                        { ReportFilter.FilterType.Include, sqlLike},
                        { ReportFilter.FilterType.NotInclude, sqlLike}
                    };
                }

                return _tBag;
            }
        }

        public Query BuildQuery(IEnumerable<ReportField> fields, IEnumerable<ReportFilter> filters, string dataSource)
        {
            //Always order report columns and filters. As result SQL will not generate different compiled plans when columns in reports have different order.
            ReportField[] reportFields = fields.OrderBy(x => x.Title).ToArray();

            string colsOrder = "";
            
            filters = filters.OrderBy(x => x.ReportFieldTitle).ThenBy(x=> x.Type).ToArray();

            foreach (var fieldDefenition in reportFields)
            {
                if (IsSqlCommaNeeded(colsOrder))
                    colsOrder += ",";

                colsOrder += fieldDefenition.SqlValueExpression + " AS " + fieldDefenition.SqlAlias;
            }

            var sqlParams = new List<IDataParameter>();
            string sqlFilter = "";
            foreach (var filter in filters)
            {
                var fieldDefenition =  reportFields.Single(x=> x.Title == filter.ReportFieldTitle);

                var formattedFilterValue = fieldDefenition.InputValueTransformation != null
                    ? fieldDefenition.InputValueTransformation(filter.Value)
                    : filter.Value;

                var parameter = QueryExecutor.GenerateDBParameter("p" + sqlParams.Count, formattedFilterValue, SqlDbType.NVarChar);
                sqlParams.Add(parameter);

                sqlFilter += " AND ";
                sqlFilter += BuildSqlFilter(filter.Type, fieldDefenition.SqlValueExpression, parameter.ParameterName);
            }

            return new Query()
            {
                SqlQuery = string.Format(SqlQueryTemplate, colsOrder, dataSource, sqlFilter),
                Parameters = sqlParams,
                Columns = reportFields.Select(x => x.SqlAlias)
            };
        }

        private static bool IsSqlCommaNeeded(string sql)
        {
            return !string.IsNullOrWhiteSpace(sql) && !sql.EndsWith(",");
        }

        private static string BuildSqlFilter(ReportFilter.FilterType filterType, string sqlValueExpression, string sqlpParameterName)
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
                case ReportFilter.FilterType.Equal:
                    sqlOperator = " = ";
                    break;
                case ReportFilter.FilterType.NotEqual:
                    sqlOperator = " != ";
                    break;
                case ReportFilter.FilterType.GreatThenOrEqualTo:
                    sqlOperator = " >= ";
                    break;
                case ReportFilter.FilterType.GreatThen:
                    sqlOperator = " > ";
                    break;
                case ReportFilter.FilterType.LessThenOrEquaslTo:
                    sqlOperator = " <= ";
                    break;
                case ReportFilter.FilterType.LessThen:
                    sqlOperator = " < ";
                    break;
                case ReportFilter.FilterType.Include:
                    sqlOperator = " like ";
                    break;
                case ReportFilter.FilterType.NotInclude:
                    sqlOperator = " not like ";
                    break;
                default:
                    sqlOperator = " = ";
                    break;
            }

            return sqlValueExpression + sqlOperator + sqlpParameterName;
        }
    }
}