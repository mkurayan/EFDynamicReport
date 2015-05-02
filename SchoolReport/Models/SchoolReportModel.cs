using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DynamicReport.Report;
using SchoolReport.Models.SchoolReportsMapping;

namespace SchoolReport.Models
{
    public class SchoolReportModel
    {
        private readonly DbContext _context;
        private readonly ReportType _reportType;

        private ReportModel Report
        {
            get
            {
                ReportModel results;

                switch (_reportType)
                {
                    case ReportType.StudentReport:
                        results = new StudentReport(_context).ReportModel;
                        break;
                    default:
                        throw new NotImplementedException(string.Format("GetReportModel not implemented for type: {0}", _reportType));
                }

                return results;
            }
        }

        public SchoolReportModel(ReportType reportType, DbContext context)
        {
            _reportType = reportType;
            _context = context;
        }

        /// <summary>
        /// Get All possible repor types.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ReportTypeDTO> GetReportTypes()
        {
            return new[]
            {
                new ReportTypeDTO {Type = ReportType.StudentReport.ToString()}
            };
        }
        
        /// <summary>
        /// Get All possible filter types.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ReportFilterDTO> GetReportFilterOperators()
        {
       
            return new[]
            {
                new ReportFilterDTO {FilterTitle = "Is equal to", Type = (int)FilterType.Equal},
                new ReportFilterDTO {FilterTitle = "Is not equal to", Type = (int)FilterType.NotEqual},
                new ReportFilterDTO {FilterTitle = "Is greater than or equal to", Type = (int)FilterType.GreatThenOrEqualTo},
                new ReportFilterDTO {FilterTitle = "Is greater than", Type = (int)FilterType.GreatThen},
                new ReportFilterDTO {FilterTitle = "Is less than or equal to", Type = (int)FilterType.LessThenOrEquaslTo},
                new ReportFilterDTO {FilterTitle = "Is less than", Type = (int)FilterType.LessThen},
                new ReportFilterDTO {FilterTitle = "Includes", Type = (int)FilterType.Include},
                new ReportFilterDTO {FilterTitle = "Not includes", Type = (int)FilterType.NotInclude},
            };
        }

        /// <summary>
        /// Get All possible report columns.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReportColumnDTO> GetReportColumns()
        {
            return Report.ReportFields.Select(x => new ReportColumnDTO { Title = x.Title, Alias = x.SqlAlias });
        }

        public List<Dictionary<string, object>> GetReportData(ReportDTO reportDto)
        {
            return Report.Get(reportDto.Columns, reportDto.Filters.Select(x => new ReportFilter()
            {
                ReportFieldTitle = x.ReportFieldTitle,
                Type = (FilterType)x.Type,
                Value = x.Value
            }));
        }
    }

    public enum ReportType
    {
        StudentReport
    }
}