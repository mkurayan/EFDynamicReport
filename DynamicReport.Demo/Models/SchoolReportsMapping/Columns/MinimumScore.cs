using DynamicReport.Demo.DB.Entities;
using DynamicReport.MappingHelpers;
using DynamicReport.Report;
using DynamicReport.SqlEngine;

namespace DynamicReport.Demo.Models.SchoolReportsMapping.Columns
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