using System;
using System.Text.RegularExpressions;

namespace DynamicReport.Report
{
    public class ReportField : IReportField
    {
        public string Title { get; set; }

        public string SqlValueExpression { get; set; }

        public string SqlAlias
        {
            get
            {
                return new Regex("[^a-zA-Z0-9]").Replace(Title, string.Empty);
            }
        }

        public Func<string, string> OutputValueTransformation { get; set; }

        public Func<string, string> InputValueTransformation { get; set; }

        /// <summary>
        /// Create empty report field. 
        /// </summary>
        public ReportField()
        {
            //set default transformations.
            OutputValueTransformation = x => x;
            InputValueTransformation = x => x;
        }
    }
}