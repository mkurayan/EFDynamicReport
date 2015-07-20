using System;
using System.Text.RegularExpressions;
using DynamicReport.MappingHelpers;
using DynamicReport.Report;
using SchoolReport.DB.Entities;

namespace SchoolReport.Models.SchoolReportsMapping.Fields
{
    public class AgeField
    {
        private static readonly Regex AgePatter = new Regex(@"\d+\s+(Years?|Months?|Days?)");

        //Outer tables
        private TableMapper<Student> S;

        public AgeField(TableMapper<Student> studentTable)
        {
            S = studentTable;
        }

        public string SqlValueExpression
        {
            get { return S.Column(x => x.DayOfBirth); }
        }

        public IReportField Field(string title)
        {
            return new ReportField
            {
                Title = title,
                SqlValueExpression = SqlValueExpression,
                OutputValueTransformation = OutputValueTransformation,
                InputValueTransformation = InputValueTransformation
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
            int years = 1900;
            int months = 0;
            int days = 0;

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
                        years = pariod;
                        break;
                    case "Month":
                    case "Months":
                        months = pariod;
                        break;
                    case "Day":
                    case "Days":
                        days = pariod;
                        break;
                }
            }

            return DateTime.UtcNow.AddYears(-1 * years).AddMonths(-1 * months).AddDays(-1 * days).ToString("yyyy-MM-dd");
        }
    }
}