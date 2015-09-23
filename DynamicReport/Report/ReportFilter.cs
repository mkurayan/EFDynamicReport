using System;
using System.Collections.Generic;

namespace DynamicReport.Report
{
    public class ReportFilter : IReportFilter
    {
        public IReportColumn ReportColumn { get; set; }

        public FilterType Type { get; set; }

        public string Value { get; set; }

        public string FormattedValue
        {
            get
            {
                return ReportColumn.InputValueTransformation != null
                    ? ReportColumn.InputValueTransformation(Value)
                    : Value;
            }  
        }

        public string BuildSqlFilter(string parameterName)
        {
            return ReportColumn.SearchConditionTransformation != null
                ? ReportColumn.SearchConditionTransformation(Value, Type, parameterName)
                : BuildDefaultSqlFilter(parameterName);
        }

        private string BuildDefaultSqlFilter(string sqlParameterName)
        {
            //Apply SQL transformation. Wrap walue to %..% symbols and so on.
            if (Bag.ContainsKey(Type))
            {
                sqlParameterName = Bag[Type](sqlParameterName);
            }

            //Wrap SQL query with 'isnull(...) check' in ordet to correct checking for inequality.
            //For more detail see http://stackoverflow.com/questions/5618357/sql-server-null-vs-empty-string
            var sqlValueExpression = "isnull((" + ReportColumn.SqlValueExpression + "),'')";

            string sqlOperator;
            switch (Type)
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

            return sqlValueExpression + sqlOperator + sqlParameterName;
        }

        private static Dictionary<FilterType, Func<string, string>> _bag;
        private static Dictionary<FilterType, Func<string, string>> Bag
        {
            get
            {
                if (_bag == null)
                {
                    Func<string, string> sqlLike = x => "'%' + " + x + " + '%'";

                    _bag = new Dictionary<FilterType, Func<string, string>>()
                    {
                        { FilterType.Include, sqlLike},
                        { FilterType.NotInclude, sqlLike}
                    };
                }

                return _bag;
            }
        }
    }
}