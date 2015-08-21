using System;
using System.Collections.Generic;
using System.Web.Http;
using DynamicReport.Demo.DB;
using DynamicReport.Demo.Models;

namespace DynamicReport.Demo.Controllers
{
    [RoutePrefix("api/reports")]
    public class ReportsController : ApiController
    {
        private readonly SchoolDbContext _context;
        public ReportsController()
        {
            _context = new SchoolDbContext();
        }

        public ReportsController(SchoolDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public IEnumerable<ReportTypeDTO> GetReportTypes()
        {
            return ReportsModel.GetReportTypes();
        }

        [HttpGet]
        [Route("filters")]
        public IEnumerable<ReportFilterDTO> GetReportFilters()
        {
            return ReportsModel.GetReportFilterOperators();
        }

        [HttpGet]
        [Route("{reportType}/columns")]
        public IEnumerable<ReportColumnDTO> GetReportColumns(string reportType)
        {
            ReportType rType = (ReportType) Enum.Parse(typeof (ReportType), reportType);

            return new ReportsModel(rType, _context).GetReportColumns();
        }

        [HttpPost]
        [Route("{reportType}")]
        public IHttpActionResult BuildNewReport([FromUri]string reportType, [FromBody]ReportDTO report)
        {
            ReportType rType = (ReportType) Enum.Parse(typeof (ReportType), reportType);
            return Json(new ReportsModel(rType, _context).GetReportData(report));
        }
    }
}