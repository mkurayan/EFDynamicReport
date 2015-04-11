using System;
using System.Collections.Generic;
using DynamicReport.Report;

namespace DynamicReport.SqlEngine
{
    public class TransformationBag
    {
        private readonly Dictionary<int, Func<string, string>> _avalilableTransformations = new Dictionary<int, Func<string, string>>();

        public string ApplyTransformation(ReportFilterType reportFilterType, ReportFieldType reportFieldType, string value)
        {
            var key = GetHashKey(reportFilterType, reportFieldType);

            string result = value;
            if (_avalilableTransformations.ContainsKey(key))
            {
                result = _avalilableTransformations[key](result);
            }

            return result;
        }

        public void AddTransformation(IEnumerable<ReportFilterType> reportFilterTypes, IEnumerable<ReportFieldType> reportFieldTypes,
           Func<string, string> transformation)
        {
            foreach (var fieldType in reportFieldTypes)
            {
                AddTransformation(reportFilterTypes, fieldType, transformation);
            }
        }

        public void AddTransformation(IEnumerable<ReportFilterType> reportFilterTypes, ReportFieldType reportFieldType,
           Func<string, string> transformation)
        {
            foreach (var filterType in reportFilterTypes)
            {
                AddTransformation(filterType, reportFieldType, transformation);
            }
        }

        public void AddTransformation(ReportFilterType reportFilterType, ReportFieldType reportFieldType,
            Func<string, string> transformation)
        {
            var key = GetHashKey(reportFilterType, reportFieldType);

            _avalilableTransformations[key] = transformation;
        }

        private static int GetHashKey(ReportFilterType reportFilterType, ReportFieldType reportFieldType)
        {
            return (int)reportFieldType * 1000 + (int)reportFilterType;
        }
    }
}