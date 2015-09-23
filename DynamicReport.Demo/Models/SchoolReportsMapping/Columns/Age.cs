using System;
using System.Text.RegularExpressions;
using DynamicReport.Demo.DB.Entities;
using DynamicReport.MappingHelpers;
using DynamicReport.Report;

namespace DynamicReport.Demo.Models.SchoolReportsMapping.Columns
{
    public class Age
    {
        //Outer tables
        private readonly TableMapper<Student> S;

        public Age(TableMapper<Student> studentTable)
        {
            S = studentTable;
        }

        public string SqlValueExpression
        {
            get { return S.Column(x => x.DayOfBirth); }
        }

        public IReportColumn Column(string title)
        {
            return new ReportColumn
            {
                Title = title,
                SqlValueExpression = SqlValueExpression,
                OutputValueTransformation = OutputValueTransformation,
                InputValueTransformation = InputValueTransformation,
                SearchConditionTransformation = GetSearchCondition
            };
        }

        private static string OutputValueTransformation(string output)
        {
            var now = DateTime.Now;
            var birthday = string.IsNullOrEmpty(output)
                ? new DateTime(1900, 0, 0, 0, 0, 0)
                : DateTime.Parse(output);

            int months = ((now.Year - birthday.Year) * 12) + now.Month - birthday.Month;

            return string.Format("{0} Years {1} Months", months / 12, months % 12);
        }

        private static string InputValueTransformation(string input)
        {
            var ageString = new AgeString(input);

            return DateTime.UtcNow.AddYears(-1 * ageString.Years).AddMonths(-1 * ageString.Months).AddDays(-1 * ageString.Days).ToString("yyyy-MM-dd");
        }

        private string GetSearchCondition(string input, FilterType filterType, string paramName)
        {
            var ageString = new AgeString(input);

            string datepart = "year";

            if (ageString.Days > 0)
            {
                datepart = "day";
            }
            else if (ageString.Months > 0)
            {
                datepart = "month";
            }

            string sqlOperator;
            switch (filterType)
            {
                case FilterType.Equal:
                case FilterType.Include:
                    sqlOperator = string.Format(" BETWEEN DATEADD({1}, -1, {0}) AND {0} ", paramName, datepart);
                    break;
                case FilterType.NotEqual:
                case FilterType.NotInclude:
                    sqlOperator = string.Format(" NOT BETWEEN DATEADD({1}, -1, {0}) AND {0} ", paramName, datepart);
                    break;
                case FilterType.GreatThenOrEqualTo:
                    sqlOperator = " <= " + paramName;
                    break;
                case FilterType.GreatThen:
                    sqlOperator = " < " + string.Format("DATEADD({1}, -1, {0})", paramName, datepart);
                    break;
                case FilterType.LessThenOrEquaslTo:
                    sqlOperator = " >= " + string.Format("DATEADD({1}, -1, {0})", paramName, datepart);
                    break;
                case FilterType.LessThen:
                    sqlOperator = " > " + paramName;
                    break;
                default:
                    throw new ReportException(string.Format("Filter type not supported: {0}", filterType));
            }

            return SqlValueExpression + sqlOperator;
        }

        /// <summary>
        /// Represent patient age.
        /// </summary>
        private class AgeString
        {
            private static readonly Regex AgePatter = new Regex(@"\d+\s+(Years?|Months?|Days?)");

            private string Age { get; set; }

            int _years;
            int _months;
            int _days;

            public AgeString(string age)
            {
                Age = age;
                ParseAge(age);
            }

            /// <summary>
            /// Years component from age string
            /// </summary>
            public int Years 
            {
                get { return _years; }
            }

            /// <summary>
            /// Months component from age string
            /// </summary>
            public int Months
            {
                get { return _months; }
            }

            /// <summary>
            /// Days component from age string
            /// </summary>
            public int Days
            {
                get { return _days; }
            }

            private void ParseAge(string input)
            {
                var matches = AgePatter.Matches(input);
                foreach (Match match in matches)
                {
                    var values = match.Value.Split(' ');

                    string interval = values[values.Length - 1];
                    int pariod = int.Parse(values[0]);

                    switch (interval)
                    {
                        case "Year":
                        case "Years":
                            _years = pariod;
                            break;
                        case "Month":
                        case "Months":
                            _months = pariod;
                            break;
                        case "Day":
                        case "Days":
                            _days = pariod;
                            break;
                    }
                }
            }
        }
    }
}