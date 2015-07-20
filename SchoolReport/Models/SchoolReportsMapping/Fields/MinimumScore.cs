using DynamicReport.MappingHelpers;
using DynamicReport.Report;
using DynamicReport.SqlEngine;
using SchoolReport.DB.Entities;

namespace SchoolReport.Models.SchoolReportsMapping.Fields
{
    public class MinimumScore
    {
        //Inner tables
        private TableMapper<ExamenResult> e;
        
        //Outer tables
        private TableMapper<Student> S;

        public MinimumScore(IQueryExtractor queryExtractor, TableMapper<Student> studentTable)
        {
            S = studentTable;
            e = new TableMapper<ExamenResult>(queryExtractor);
        }

        public string SqlValueExpression
        {
            get
            {
                return 
                    " SELECT MIN(" + e.Column(x => x.Score) + ") " + 
                    " FROM " + e.Table() + 
                    " WHERE " + e.Column(x => x.StudentId) + " = " + S.Column(x => x.StudentId);
            }
        }

        public IReportField Field(string title)
        {
            return new ReportField
            {
                Title = title,
                SqlValueExpression = SqlValueExpression
            };
        }
    }
}