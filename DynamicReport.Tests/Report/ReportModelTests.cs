using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DynamicReport.Report;
using DynamicReport.SqlEngine;
using Moq;
using NUnit.Framework;

namespace DynamicReport.Tests.Report
{
    [TestFixture]
    class ReportModelTests
    {
        private ReportModel _reportModel; 

        [SetUp]
        public void SetUp()
        {
            _reportModel = new ReportModel(new Mock<IQueryBuilder>().Object, new Mock<IQueryExecutor>().Object);
        }

        [Test]
        public void AddReportField_GivenNullValue_ThrownArgumentNullException()
        {
            Assert.Throws(typeof (ArgumentNullException),
                () =>
                {
                    _reportModel.AddReportField(null);
                });
        }

        [Test]
        public void AddReportField_GivenDuplicateFields_ThrownArgumentNullException()
        {
            _reportModel.AddReportField(new ReportField(){Title = "Field A"});

            Assert.Throws(typeof(ReportException),
                () =>
                {
                    _reportModel.AddReportField(new ReportField() { Title = "Field A" });
                });
        }

        [Test]
        public void AddReportField_GivenSeveralValidFields_FieldsSuccessfullyAddedToReportModel()
        {
            _reportModel.AddReportField(new ReportField() { Title = "Field A" });
            _reportModel.AddReportField(new ReportField() { Title = "Field B" });

            Assert.That(_reportModel.ReportFields.Count(), Is.EqualTo(2));

            Assert.That(_reportModel.ReportFields.First().Title, Is.EqualTo("Field A"));
            Assert.That(_reportModel.ReportFields.Last().Title, Is.EqualTo("Field B"));
        }

        [Test]
        public void GetReportField_GivenReportFieldTitle_ReportFieldReturned()
        {
            _reportModel.AddReportField(new ReportField() { Title = "Field A" });
            _reportModel.AddReportField(new ReportField() { Title = "Field B" });

            var reportField = _reportModel.GetReportField("Field A");
            
            Assert.NotNull(reportField);
            Assert.That(reportField.Title, Is.EqualTo("Field A"));
        }

        [Test]
        public void GetReportField_GivenUnknownReportFieldTitle_NullReturned()
        {
            _reportModel.AddReportField(new ReportField() { Title = "Field A" });
            _reportModel.AddReportField(new ReportField() { Title = "Field B" });

            var reportField = _reportModel.GetReportField("Field C");

            Assert.Null(reportField);
        }


        [Test]
        public void SetDataSource_GivenNullValue_ThrownArgumentNullException()
        {
            Assert.Throws(typeof(ArgumentNullException),
               () =>
               {
                   _reportModel.SetDataSource(null);
               });
        }

        [Test]
        public void SetDataSource_GivenEmptyDataSource_ThrownReportException()
        {
            var dataSource = new Mock<IReportDataSource>();
            dataSource.SetupGet(x => x.SqlQuery).Returns(string.Empty);

            Assert.Throws(typeof(ReportException),
               () =>
               {
                   _reportModel.SetDataSource(dataSource.Object);
               });
        }

        [Test]
        public void SetDataSource_GivenValidDataSource_SetUpDataSource()
        {
            var dataSource = new ReportDataSource("select * from xxx");
            _reportModel.SetDataSource(dataSource);
        }

        [Test]
        public void Get_NullAsArguments_ThrownArgumentNulException()
        {
            Assert.Throws(typeof(ArgumentNullException),
              () =>
              {
                  _reportModel.Get(null, Enumerable.Empty<IReportFilter>());
              });

            Assert.Throws(typeof(ArgumentNullException),
              () =>
              {
                  _reportModel.Get(Enumerable.Empty<IReportField>(), null);
              });
        }

        [Test]
        public void Validate_GivenEmptyReportFieldsColection_ThrownReportException()
        {
            _reportModel.AddReportField(new ReportField()
            {
                Title = "Field A",
                SqlValueExpression = "select A"
            });
            _reportModel.SetDataSource(new ReportDataSource("select * from ..."));


            var fields = Enumerable.Empty<IReportField>();
            var filters = Enumerable.Empty<IReportFilter>();

            const string expectedError = "Report must have at least one output column.";
            
            var errors = _reportModel.Validate(fields, filters);

            Assert.That(errors.Length, Is.EqualTo(1));
            Assert.That(errors[0], Is.EqualTo(expectedError));

            var exception = Assert.Throws(typeof(ReportException),
              () =>
              {
                  _reportModel.Get(fields, filters);
              });

            Assert.True(exception.Message.Contains(expectedError));
        }

        [Test]
        public void Validate_GivenUnknownReportField_ThrownReportException()
        {
            //Set collection of available for report fields
            _reportModel.AddReportField(new ReportField()
            {
                Title = "Field A",
                SqlValueExpression = "select A"
            });
            _reportModel.SetDataSource(new ReportDataSource("select * from ..."));

            var fields = new List<IReportField>()
            {
                new ReportField()
                {
                    Title = "Unknown Field",
                    SqlValueExpression = "select B"
                }
            };
            var filters = Enumerable.Empty<IReportFilter>();

            const string expectedError = "Unknow report filed: Unknown Field";

            var errors = _reportModel.Validate(fields, filters);

            Assert.That(errors.Length, Is.EqualTo(1));
            Assert.That(errors[0], Is.EqualTo(expectedError));

            var exception = Assert.Throws(typeof(ReportException),
              () =>
              {
                  _reportModel.Get(fields, filters);
              });

            Assert.True(exception.Message.Contains(expectedError));
        }

        [Test]
        public void Validate_GivenUnknownReportFilter_ThrownReportException()
        {
            //Set collection of available for report fields
            _reportModel.AddReportField(new ReportField()
            {
                Title = "Field A",
                SqlValueExpression = "select A"
            });
            _reportModel.SetDataSource(new ReportDataSource("select * from ..."));


            var fields = new List<IReportField>()
            {
                new ReportField()
                {
                    Title = "Field A",
                    SqlValueExpression = "select A"
                }
            };
            var filters = new List<IReportFilter>()
            {
                new ReportFilter()
                {
                    ReportField = new ReportField()
                    {
                        Title = "Unknown Field",
                        SqlValueExpression = "select B"
                    },
                    Type = FilterType.Equal,
                    Value = "Some value"
                }
            };

            const string expectedError = "Unknow report filter, field: Unknown Field";

            var errors = _reportModel.Validate(fields, filters);

            Assert.That(errors.Length, Is.EqualTo(1));
            Assert.That(errors[0], Is.EqualTo(expectedError));

            var exception = Assert.Throws(typeof(ReportException),
              () =>
              {
                  _reportModel.Get(fields, filters);
              });

            Assert.True(exception.Message.Contains(expectedError));
        }

        [Test]
        public void Validate_ReportModelEmpty_ThrownReportException()
        {

        }

        [Test]
        public void Get_ValidInputs_ReportReturned()
        {
            var fields = new List<IReportField>()
            {
                new ReportField()
                {
                    Title = "Field A",
                    SqlValueExpression = "select A"
                }
            };
            var filters = Enumerable.Empty<IReportFilter>();
            string dataSource = "select * from ...";

            var sqlCommand = new SqlCommand("select FieldA from ....");
            DataTable reportTable = new DataTable();
            reportTable.Columns.Add("FieldA");
            reportTable.Rows.Add("Row #1");
            reportTable.Rows.Add("Row #2");

            var queryBuilder = new Mock<IQueryBuilder>();
            queryBuilder.Setup(x => x.BuildQuery(fields, filters, dataSource)).Returns(sqlCommand);

            var queryExecutor = new Mock<IQueryExecutor>();
            queryExecutor.Setup(x => x.ExecuteToDataTable(sqlCommand)).Returns(reportTable);

            var reportModel = new ReportModel(queryBuilder.Object, queryExecutor.Object);
            reportModel.AddReportField(fields.First());
            reportModel.SetDataSource(new ReportDataSource("select * from ..."));

            var result = reportModel.Get(fields, filters);

            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(result[0].Count, Is.EqualTo(1));
            Assert.True(result[0].ContainsKey("FieldA"));
            Assert.That(result[0]["FieldA"], Is.EqualTo("Row #1"));

            Assert.That(result[1].Count, Is.EqualTo(1));
            Assert.True(result[1].ContainsKey("FieldA"));
            Assert.That(result[1]["FieldA"], Is.EqualTo("Row #2"));
        }
    }
}
