using System;
using System.Collections.Generic;
using System.Web.Http;
using SchoolReport.DB;
using SchoolReport.Models;

namespace SchoolReport.Controllers
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
        //[Route("reports")]
        [Route("")]
        public IEnumerable<ReportTypeDTO> GetReportTypes()
        {
            return SchoolReportModel.GetReportTypes();
        }

        [HttpGet]
        [Route("filters")]
        public IEnumerable<ReportFilterDTO> GetReportFilters()
        {
            return SchoolReportModel.GetReportFilterOperators();
        }

        [HttpGet]
        [Route("{reportType}/columns")]
        public IEnumerable<ReportColumnDTO> GetReportColumns(string reportType)
        {
            ReportType rType = (ReportType) Enum.Parse(typeof (ReportType), reportType);

            return new SchoolReportModel(rType, _context).GetReportColumns();
        }

        [HttpPost]
        [Route("{reportType}")]
        public IHttpActionResult BuildNewReport([FromUri]string reportType, [FromBody]ReportDTO report)
        {
            ReportType rType = (ReportType) Enum.Parse(typeof (ReportType), reportType);
            return Json(new SchoolReportModel(rType, _context).GetReportData(report));
        }
    }
}