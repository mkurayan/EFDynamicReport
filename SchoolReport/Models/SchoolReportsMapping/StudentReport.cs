using System.Collections.Generic;
using System.Data.Entity;
using DynamicReport.Mapping;
using DynamicReport.Report;
using SchoolReport.DB.Entities;

namespace SchoolReport.Models.SchoolReportsMapping
{
    public class StudentReport : ReportMapper
    {
        public StudentReport(DbContext context) : base(context)
        {
        }

        protected override IEnumerable<FieldMapper> ReportFields
        {
            get
            {
                return new List<FieldMapper>
                {
                    FieldFromLambda("First Name", (Student x) => x.FirstName),
                    FieldFromLambda("Last Name", (Student x) => x.LastName),
                    FieldFromLambda("Phone", (Student x)=> x.Phone),
                    FieldFromLambda("Home Adress", (Student x) => x.HomeAdress)
                };
            }
        }

        protected override DataSourceMapper DataSource
        {
            get { return FromLambda((Student x) => x); }
        }

        public ReportModel ReportModel
        {
            get { return BuildReportModel(); }
        }
    }
}