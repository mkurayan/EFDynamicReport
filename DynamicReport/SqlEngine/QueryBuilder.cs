using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DynamicReport.Report;

namespace DynamicReport.SqlEngine
{
    public class QueryBuilder
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

            string sqlQuery = string.Format("SELECT {0} FROM {1}", colsOrder, dataSource);
            if (!string.IsNullOrEmpty(sqlFilter))
            {
                sqlQuery += string.Format(" WHERE {0}", sqlFilter);
            }

            return new Query()
            {
                SqlQuery = sqlQuery,
                Parameters = sqlParams,
                Columns = reportFields.Select(x => x.SqlAlias)
            };
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
    }
}