using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicReport.Report;
using NUnit.Framework;

namespace DynamicReport.Tests.Report
{
    [TestFixture]
    class ReportFilterTests
    {
        [Test]
        public void FormattedValue_InputValueTransformationNotProvided_FilterValue()
        {
            var initialValue = "1";

            ReportFilter filter = new ReportFilter()
            {
                ReportColumn = new ReportColumn()
                {
                    Title = "Test column",
                    SqlValueExpression = "(foo.TestColumn)",
                    InputValueTransformation = null
                },
                Type = FilterType.Equal,
                Value = initialValue
            };

            Assert.That(filter.FormattedValue, Is.EqualTo(initialValue));
        }

        [Test]
        public void FormattedValue_GivenInputValueTransformation_FilterValueFormatted()
        {
            var initialValue = "1";
            var expectedValue = "1_1";

            ReportFilter filter = new ReportFilter()
            {
                ReportColumn = new ReportColumn()
                {
                    Title = "Test column",
                    SqlValueExpression = "(foo.TestColumn)",
                    InputValueTransformation = x => x + "_" + x
                },
                Type = FilterType.Equal,
                Value = initialValue
            };

            Assert.That(filter.FormattedValue, Is.EqualTo(expectedValue));
        }

        [TestCase(FilterType.Equal, "isnull((TestTable.fField),'') = @p0")]
        [TestCase(FilterType.NotEqual, "isnull((TestTable.fField),'') != @p0")]
        [TestCase(FilterType.GreatThen, "isnull((TestTable.fField),'') > @p0")]
        [TestCase(FilterType.GreatThenOrEqualTo, "isnull((TestTable.fField),'') >= @p0")]
        [TestCase(FilterType.LessThen, "isnull((TestTable.fField),'') < @p0")]
        [TestCase(FilterType.LessThenOrEquaslTo, "isnull((TestTable.fField),'') <= @p0")]
        [TestCase(FilterType.Include, "isnull((TestTable.fField),'') like '%' + @p0 + '%'")]
        [TestCase(FilterType.NotInclude, "isnull((TestTable.fField),'') not like '%' + @p0 + '%'")]
        public void BuildSqlFilter_SearchConditionTransformationNotProvided_DefaultFilterExpression(FilterType filterType, string expectedSqlExpression)
        {
            ReportFilter filter = new ReportFilter()
            {
                Type = filterType,
                ReportColumn = new ReportColumn
                {
                    Title = "First Field",
                    SqlValueExpression = "TestTable.fField",
                },
            };

            var sqlFilter = filter.BuildSqlFilter("@p0");

            Assert.That(sqlFilter, Is.EqualTo(expectedSqlExpression));
        }

        [TestCase(FilterType.Equal, "@p0 = a")]
        [TestCase(FilterType.NotEqual, "@p0 != a")]
        public void BuildSqlFilter_GivenSearchConditionTransformation_CustomFilterExpression(FilterType filterType, string expectedSqlExpression)
        {
            ReportFilter filter = new ReportFilter()
            {
                Value = "a",
                Type = filterType,
                ReportColumn = new ReportColumn
                {
                    Title = "First Field",
                    SqlValueExpression = "TestTable.fField",
                    SearchConditionTransformation = (fValue, y, parameterName) =>
                    {
                        string result = string.Empty;

                        switch (y)
                        {
                            case FilterType.Equal:
                                result = parameterName + " = " + fValue;
                                break;
                            case FilterType.NotEqual:
                                result = parameterName + " != " + fValue;
                                break;
                        }

                        return result;
                    } 
                },
            };

            var sqlFilter = filter.BuildSqlFilter("@p0");

            Assert.That(sqlFilter, Is.EqualTo(expectedSqlExpression));
        }

    }
}
