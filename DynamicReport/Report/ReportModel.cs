using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using DynamicReport.SqlEngine;

namespace DynamicReport.Report
{
    public class ReportModel
    {
        /// <summary>
        /// Set of available fields in report.
        /// </summary>
        public ReportField[] AvailableFields { get; private set; }

        /// <summary>
        /// Report SQL query.
        /// </summary>
        /// <returns></returns>
        public string SqlQuery { get; private set; }

        public ReportModel(IEnumerable<ReportField> availableFields, string sqlQuery)
        {
            var error = ValidateFieldsDefinitions(availableFields);
            if (!string.IsNullOrEmpty(error))
            {
                throw new ArgumentException(error, "availableFields");
            }

            SqlQuery = sqlQuery;

            AvailableFields = availableFields.ToArray();
        }
        //ToDo: hospitalId
        /// <summary>
        /// Process report with proposed fields and filters, return report data.
        /// </summary>
        /// <param name="columns">Fields which will be included in report.</param>
        /// <param name="filters">Filters which will be applied on report.</param>
        /// <param name="hospitalId">Id of hospital for which report will be build.</param>
        public List<Dictionary<string, object>> Get(string[] columns, ReportFilter[] filters, int hospitalId)
        {
            var error = Validate(columns, filters);
            if (!string.IsNullOrEmpty(error))
            {
                throw new ReportException(error);
            }

            var query = 
                new QueryBuilder()
                    .BuildQuery(columns.Select(x => AvailableFields.Single(y => y.Title == x)), filters, SqlQuery, hospitalId);

            var data =
                new QueryExecutor(ConfigurationManager.ConnectionStrings["SnapConn"].ConnectionString)
                    .ExecuteToDataTable(query);

            return GetJson(data);
        }

        private List<Dictionary<string, object>> GetJson(DataTable dt)
        {
            var rows = new List<Dictionary<string, object>>();

            var reportFields = new ReportField[dt.Columns.Count];

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                reportFields[i] = AvailableFields.Single(x => x.SqlAlias == dt.Columns[i].ColumnName);
            }

            foreach (DataRow dr in dt.Rows)
            {
                var row = new Dictionary<string, object>();

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string value = "";
                    if (dr[j] != null)
                    {
                        value = reportFields[j].OutputValueTransformation(dr[j].ToString());
                    }

                    row.Add(dt.Columns[j].ColumnName, value);
                }

                rows.Add(row);
            }

            return rows;
        }

        /// <summary>
        /// Check that report can process specified fields and filters.
        /// </summary>
        /// <param name="columnsTitles">Fields which proposed for report.</param>
        /// <param name="filters">Filters which proposed for report.</param>
        /// <returns>Validation result, null if there is no validation errors.</returns>
        private string Validate(string[] columnsTitles, IEnumerable<ReportFilter> filters)
        {
            var errors = new List<string>();

            if (!columnsTitles.Any())
            {
                errors.Add("Report must have at least one output column.");
            }

            foreach (var field in columnsTitles)
            {
                if (AvailableFields.All(x => x.Title != field))
                {
                    errors.Add("Unknow report filed: " + field);
                }
            }

            foreach (var filter in filters)
            {
                if (AvailableFields.All(x => x.Title != filter.ReportFieldTitle))
                {
                    errors.Add("Unknow report filter, field: " + filter.ReportFieldTitle);
                }
            }

            if (!errors.Any()) return null;

            StringBuilder error = new StringBuilder();
            error.AppendLine("Value of a fields or filters column is outside the allowable range of values.");
            foreach (var err in errors)
            {
                error.AppendLine(err);
            }

            return error.ToString();
        }

        private string ValidateFieldsDefinitions(IEnumerable<ReportField> reportFieldDefinitions)
        {
            if (reportFieldDefinitions == null)
            {
                return "Null";
            }

            if (!reportFieldDefinitions.Any())
            {
                return "Must contain at least one available field.";
            }

            var duplicates = reportFieldDefinitions.GroupBy(x => x.Title).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicates.Any())
            {
                return string.Format("Report model can not contain duplicate fields: {0}",string.Join(",", duplicates));
            }

            return null;
        }     
    }
}