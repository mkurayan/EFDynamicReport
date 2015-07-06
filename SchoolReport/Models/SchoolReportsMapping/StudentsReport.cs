using System;
using System.Collections.Generic;
using System.Data.Entity;
using DynamicReport.Mapping;
using DynamicReport.Report;
using DynamicReport.SqlEngine;
using SchoolReport.DB.Entities;

namespace SchoolReport.Models.SchoolReportsMapping
{
    public class StudentsReport : ReportTemplate
    {
        public TableMapper<Student> StudentTable;

        public StudentsReport(DbContext context) : base(context)
        {
            StudentTable = new TableMapper<Student>(new EFMappingExtractor(context), "s");
        }

        public override IEnumerable<ReportField> ReportFields
        {
            get
            {
                return new List<ReportField>
                {
                    StudentTable.Field("First Name", x => x.FirstName),
                    StudentTable.Field("Last Name", x => x.LastName),
                    StudentTable.Field("Phone", x => x.Phone),
                    StudentTable.Field("Home Adress", x => x.HomeAdress)
                };
            }
        }

        protected override ReportDataSource ReportDataSource
        {
            get { return StudentTable.GetReportDataSource(); }
        }
    }
}