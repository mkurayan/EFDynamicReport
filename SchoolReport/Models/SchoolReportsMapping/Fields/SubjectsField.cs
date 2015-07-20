using DynamicReport.MappingHelpers;
using DynamicReport.Report;
using DynamicReport.SqlEngine;
using SchoolReport.DB.Entities;

namespace SchoolReport.Models.SchoolReportsMapping.Fields
{
    public class SubjectsField
    {
        //Inner tables
        private TableMapper<Subject> sb;
        private TableMapper<ExamenResult> e; 

        //Outer tables
        private TableMapper<Student> S;

        public SubjectsField(IQueryExtractor queryExtractor, TableMapper<Student> studentTable)
        {
            sb = new TableMapper<Subject>(queryExtractor);
            e = new TableMapper<ExamenResult>(queryExtractor);
            S = studentTable;
        }

        public string SqlValueExpression
        {
            get
            {
                return "substring(  " +
                       "(SELECT ', ' + " + sb.Column(s => s.SubjectName) +
                       " FROM " + sb.Table() +
                       " INNER JOIN " + e.Table() + " ON " + e.Column(x => x.SubjectId) + " = " + sb.Column(x => x.SubjectId) +
                       " WHERE " + e.Column(x => x.StudentId) + " = " + S.Column(x => x.StudentId) +
                       " FOR XML PATH ('') ), 2, 1000)";
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