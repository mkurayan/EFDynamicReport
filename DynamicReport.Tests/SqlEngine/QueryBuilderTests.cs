using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DynamicReport.Report;
using DynamicReport.SqlEngine;
using NUnit.Framework;

namespace DynamicReport.Tests.SqlEngine
{
    [TestFixture]
    class QueryBuilderTests
    {
        private QueryBuilder queryBuilder;
        private IEnumerable<IReportField> _reportFields;
        private string _dataSource = "TestTable";
            
        [SetUp]
        public void SetUp()
        {
           queryBuilder = new QueryBuilder();

           _reportFields = new IReportField[]
           {
               new ReportField
               {
                   Title = "First Field",
                   SqlValueExpression = "TestTable.fField",
               }
           };
        }

        [Test]
        public void BuildQuery_ReportFieldsCollectionNullOrEmpty_ReportExceptionThrown()
        {
            Assert.Throws(typeof (ReportException),
                () => queryBuilder.BuildQuery(null, Enumerable.Empty<IReportFilter>(), _dataSource));
        }

        [Test]
        public void BuildQuery_EmptyReportFieldsCollection_ReportExceptionThrown()
        {
            Assert.Throws(typeof (ReportException),
                () => queryBuilder.BuildQuery(Enumerable.Empty<IReportField>(), Enumerable.Empty<IReportFilter>(), _dataSource));
        }

        [TestCase(null)]
        [TestCase("")]
        public void BuildQuery_ReportDataSourceNotProvided_ReportExceptionThrown(string dataSource)
        {
             Assert.Throws(typeof (ReportException),
                 () => queryBuilder.BuildQuery(_reportFields, Enumerable.Empty<IReportFilter>(), dataSource));
        }

        [Test]
        public void BuildQuery_GivenSingleReportField_ValidSqlCommand()
        {
            SqlCommand command = queryBuilder.BuildQuery(_reportFields, Enumerable.Empty<IReportFilter>(), _dataSource);

            Assert.That(command.CommandText, Is.EqualTo("SELECT (TestTable.fField) AS FirstField FROM TestTable"));
            Assert.That(command.Parameters.Count, Is.EqualTo(0));
        }

        [Test]
        public void BuildQuery_GivenMultipleReportFields_ValidSqlCommand()
        {
            var reportFields = new IReportField[]
            {
                new ReportField
                {
                    Title = "First Field",
                    SqlValueExpression = "TestTable.fField",
                },
                new ReportField
                {
                    Title = "Second Field",
                    SqlValueExpression = "TestTable.sField",
                },
            };

            SqlCommand command = queryBuilder.BuildQuery(reportFields, Enumerable.Empty<IReportFilter>(), _dataSource);

            Assert.That(command.CommandText, Is.EqualTo("SELECT (TestTable.fField) AS FirstField, (TestTable.sField) AS SecondField FROM TestTable"));
            Assert.That(command.Parameters.Count, Is.EqualTo(0));
        }

        [TestCase(FilterType.Equal, "isnull(TestTable.fField,'') = @p0")]
        [TestCase(FilterType.NotEqual, "isnull(TestTable.fField,'') != @p0")]
        [TestCase(FilterType.GreatThen, "isnull(TestTable.fField,'') > @p0")]
        [TestCase(FilterType.GreatThenOrEqualTo, "isnull(TestTable.fField,'') >= @p0")]
        [TestCase(FilterType.LessThen, "isnull(TestTable.fField,'') < @p0")]
        [TestCase(FilterType.LessThenOrEquaslTo, "isnull(TestTable.fField,'') <= @p0")]
        [TestCase(FilterType.Include, "isnull(TestTable.fField,'') like '%' + @p0 + '%'")]
        [TestCase(FilterType.NotInclude, "isnull(TestTable.fField,'') not like '%' + @p0 + '%'")]
        public void BuildQuery_GivenSingleFilter__ValidSqlCommand(FilterType filterType, string expectedSqlExpression)
        {
            var reportFilters = new IReportFilter[]
            {
                new ReportFilter
                {
                    ReportField = new ReportField
                    {
                        Title = "First Field",
                        SqlValueExpression = "TestTable.fField",
                    },
                    Type = filterType,
                    Value = "SomeValue"
                }
            };

            SqlCommand command = queryBuilder.BuildQuery(_reportFields, reportFilters, _dataSource);

            var expectedSql = string.Format("SELECT (TestTable.fField) AS FirstField FROM TestTable WHERE {0}", expectedSqlExpression);

            Assert.That(command.Parameters.Count, Is.EqualTo(1));
            Assert.That(command.Parameters[0].Value, Is.EqualTo("SomeValue"));
            Assert.That(command.Parameters[0].ParameterName, Is.EqualTo("@p0"));

            Assert.That(command.CommandText, Is.EqualTo(expectedSql));
        }

        [Test]
        public void BuildQuery_GivenMultipleFilters__ValidSqlCommand()
        {
            var reportFilters = new IReportFilter[]
            {
                new ReportFilter
                {
                    ReportField = new ReportField
                    {
                        Title = "First Field",
                        SqlValueExpression = "TestTable.fField"
                    },
                    Type = FilterType.NotEqual,
                    Value = "SomeValue"
                },

                new ReportFilter
                {
                    ReportField = new ReportField
                    {
                        Title = "First Field",
                        SqlValueExpression = "TestTable.fField"
                    },
                    Type = FilterType.NotEqual,
                    Value = "AnotherValue"
                }
            };

            SqlCommand command = queryBuilder.BuildQuery(_reportFields, reportFilters, _dataSource);

            var expectedSql = "SELECT (TestTable.fField) AS FirstField FROM TestTable WHERE isnull(TestTable.fField,'') != @p0 AND isnull(TestTable.fField,'') != @p1";

            Assert.That(command.Parameters.Count, Is.EqualTo(2));
            Assert.That(command.Parameters[0].Value, Is.EqualTo("SomeValue"));
            Assert.That(command.Parameters[0].ParameterName, Is.EqualTo("@p0"));

            Assert.That(command.Parameters[1].Value, Is.EqualTo("AnotherValue"));
            Assert.That(command.Parameters[1].ParameterName, Is.EqualTo("@p1"));

            Assert.That(command.CommandText, Is.EqualTo(expectedSql));
        }
    }
}
