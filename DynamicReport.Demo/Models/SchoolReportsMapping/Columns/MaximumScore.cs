using DynamicReport.Demo.DB.Entities;
using DynamicReport.MappingHelpers;
using DynamicReport.Report;
using DynamicReport.SqlEngine;

namespace DynamicReport.Demo.Models.SchoolReportsMapping.Columns
{
    public class MaximumScore
    {
        //Inner tables
        private readonly TableMapper<ExamenResult> e;
        
        //Outer tables
        private readonly TableMapper<Student> S;

        public MaximumScore(IQueryExtractor queryExtractor, TableMapper<Student> studentTable)
        {
            S = studentTable;
            e = new TableMapper<ExamenResult>(queryExtractor, "max");
        }

        public string SqlValueExpression
        {
            get
            {
                return 
                    " SELECT MAX(" + e.Column(x => x.Score) + ") " + 
                    " FROM " + e.Table() + 
                    " WHERE " + e.Column(x => x.StudentId) + " = " + S.Column(x => x.StudentId);
            }
        }

        public IReportColumn Column(string title)
        {
            return new ReportColumn
            {
                Title = title,
                SqlValueExpression = SqlValueExpression
            };
        }
    }
}