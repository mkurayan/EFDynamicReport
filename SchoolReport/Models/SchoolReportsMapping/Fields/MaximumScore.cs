using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicReport.Mapping;
using DynamicReport.Report;
using DynamicReport.SqlEngine;
using SchoolReport.DB.Entities;

namespace SchoolReport.Models.SchoolReportsMapping.Fields
{
    public class MaximumScore
    {
        //Inner tables
        private TableMapper<ExamenResult> e;
        
        //Outer tables
        private TableMapper<Student> S;

        public MaximumScore(EFMappingExtractor efMappingExtractor, TableMapper<Student> studentTable)
        {
            S = studentTable;
            e = new TableMapper<ExamenResult>(efMappingExtractor);
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

        public ReportField Field(string title)
        {
            return new ReportField
            {
                Title = title,
                SqlValueExpression = SqlValueExpression
            };
        }
    }
}