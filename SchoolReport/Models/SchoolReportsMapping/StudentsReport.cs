using System;
using System.Collections.Generic;
using System.Data.Entity;
using DynamicReport.Mapping;
using DynamicReport.Report;
using DynamicReport.SqlEngine;
using SchoolReport.DB.Entities;
using SchoolReport.Models.SchoolReportsMapping.Fields;

namespace SchoolReport.Models.SchoolReportsMapping
{
    public class StudentsReport : ReportTemplate
    {
        private EFMappingExtractor efMappingExtractor;
        public TableMapper<Student> StudentTable;

        public StudentsReport(DbContext context) : base(context)
        {
            efMappingExtractor = new EFMappingExtractor(context);
            StudentTable = new TableMapper<Student>(efMappingExtractor, "s");
        }

        private IEnumerable<ReportField> _reportFields;
        public override IEnumerable<ReportField> ReportFields
        {
            get
            {
                if (_reportFields == null)
                {
                    _reportFields = new List<ReportField>
                    {
                        StudentTable.Field("First Name", x => x.FirstName),
                        StudentTable.Field("Last Name", x => x.LastName),
                        StudentTable.Field("Phone", x => x.Phone),
                        StudentTable.Field("Home Adress", x => x.HomeAdress),
                        new AverageScore(efMappingExtractor, StudentTable).Field("Average Score"),
                        new MinimumScore(efMappingExtractor, StudentTable).Field("Minimum Score"),
                        new MaximumScore(efMappingExtractor, StudentTable).Field("Maximum Score"),
                        new AgeField(StudentTable).Field("Age"),
                        new SubjectsField(efMappingExtractor, StudentTable).Field("Subjects")
                    };
                }

                return _reportFields;
            }
        }

        protected override ReportDataSource ReportDataSource
        {
            get { return StudentTable.GetReportDataSource(); }
        }
    }
}