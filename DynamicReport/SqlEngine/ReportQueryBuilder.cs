using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using DynamicReport.Report;

namespace DynamicReport.SqlEngine
{
    public class ReportQueryBuilder
    {
        private static TransformationBag _tBag;
        private static TransformationBag TBag
        {
            get
            {
                if (_tBag == null)
                {
                    var comparableFilters = new[]
                    {
                        ReportFilterType.Equal, ReportFilterType.GreatThen, ReportFilterType.GreatThenOrEqualTo,
                        ReportFilterType.LessThen, ReportFilterType.LessThenOrEquaslTo, ReportFilterType.NotEqual
                    };

                    var notComparableFilters = new[]
                    {
                        ReportFilterType.Include, ReportFilterType.NotInclude
                    };

                    _tBag = new TransformationBag();

                    //Wrap parameter value inside %...%. Example: WHERE Name LIKE '%' + @PartialName + '%'                
                    _tBag.AddTransformation(notComparableFilters, new[] { ReportFieldType.General, ReportFieldType.Date, ReportFieldType.Time }, (x => "'%' + " + x + " + '%'"));

                    _tBag.AddTransformation(comparableFilters, ReportFieldType.Time, (x => string.Format("CONVERT(varchar,{0},100)", x)));

                    _tBag.AddTransformation(comparableFilters, ReportFieldType.Date, (x => string.Format("CONVERT(varchar,{0},107)", x)));
                }

                return _tBag;
            }
        }

        public DataTable GetDataFromDB(IEnumerable<ReportField> fields, ReportFilter[] filters, string sqlQuery, int hospitalId)
        {
            //Always order report columns and filters. As result SQL will not generate different compiled plans when columns in reports have different order.
            ReportField[] reportFields = fields.OrderBy(x => x.Title).ToArray();

            string colsOrder = "";
            
            filters = filters.OrderBy(x => x.ReportFieldTitle).ThenBy(x=> x.FilterType).ToArray();

            foreach (var fieldDefenition in reportFields)
            {
                if (IsSqlCommaNeeded(colsOrder))
                    colsOrder += ",";

                colsOrder += TBag.ApplyTransformation(ReportFilterType.Equal, fieldDefenition.ReportFieldType, fieldDefenition.SqlValueExpression) + " AS " + fieldDefenition.SqlAlias;
            }

            //Add default parameters which always uses in reports.
            var sqlParams = new List<IDataParameter>
            {
                QueryExecutor.GenerateDBParameter("HospitalID", hospitalId, SqlDbType.Int)
            };

            string sqlFilter = "";

            for (int i = 0; i < filters.Count(); i++)
            {
                var filter = filters[i];

                var fieldDefenition =  reportFields.Single(x=> x.Title == filter.ReportFieldTitle);

                var parameter = QueryExecutor.GenerateDBParameter("p" + i, filter.FormattedValue, SqlDbType.NVarChar);
                sqlParams.Add(parameter);

                sqlFilter += " AND ";
                sqlFilter += fieldDefenition.GetSearchCondition(filter.Value, filter.FilterType, parameter.ParameterName) ?? BuildSqlFilter(filter.FilterType, fieldDefenition.ReportFieldType, fieldDefenition.SqlValueExpression, parameter.ParameterName);
            }

            var sqlAllData = string.Format(sqlQuery, colsOrder, sqlFilter);

            return new QueryExecutor(ConfigurationManager.ConnectionStrings["SnapConn"].ConnectionString).ExecuteToDataTable(sqlAllData, sqlParams.ToArray(), reportFields.Select(x=> x.SqlAlias));
        }

        private static bool IsSqlCommaNeeded(string sql)
        {
            return !string.IsNullOrWhiteSpace(sql) && !sql.EndsWith(",");
        }

        private static string BuildSqlFilter(ReportFilterType filterType, ReportFieldType reportFieldType, string sqlValueExpression, string sqlpParameterName)
        {
            //Apply SQL transformation. Wrap walue to %..% symbols and so on.
            sqlpParameterName = TBag.ApplyTransformation(filterType, reportFieldType, sqlpParameterName);

            //Wrap SQL query with 'isnull(...) check' in ordet to correct checking for inequality.
            //For more detail see http://stackoverflow.com/questions/5618357/sql-server-null-vs-empty-string
            sqlValueExpression = "isnull(" + sqlValueExpression + ",'')";

            string sqlOperator;
            switch (filterType)
            {
                case ReportFilterType.Equal:
                    sqlOperator = " = ";
                    break;
                case ReportFilterType.NotEqual:
                    sqlOperator = " != ";
                    break;
                case ReportFilterType.GreatThenOrEqualTo:
                    sqlOperator = " >= ";
                    break;
                case ReportFilterType.GreatThen:
                    sqlOperator = " > ";
                    break;
                case ReportFilterType.LessThenOrEquaslTo:
                    sqlOperator = " <= ";
                    break;
                case ReportFilterType.LessThen:
                    sqlOperator = " < ";
                    break;
                case ReportFilterType.Include:
                    sqlOperator = " like ";
                    break;
                case ReportFilterType.NotInclude:
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