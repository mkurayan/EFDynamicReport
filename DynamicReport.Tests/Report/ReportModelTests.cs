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
        public void AddReportColumn_GivenNullValue_ThrownArgumentNullException()
        {
            Assert.Throws(typeof (ArgumentNullException),
                () =>
                {
                    _reportModel.AddReportColumn(null);
                });
        }

        [Test]
        public void AddReportColumn_GivenDuplicateColumns_ThrownArgumentNullException()
        {
            _reportModel.AddReportColumn(new ReportColumn(){Title = "Column A"});

            Assert.Throws(typeof(ReportException),
                () =>
                {
                    _reportModel.AddReportColumn(new ReportColumn() { Title = "Column A" });
                });
        }

        [Test]
        public void AddReportColumn_GivenSeveralValidColumns_ColumnsSuccessfullyAddedToReportModel()
        {
            _reportModel.AddReportColumn(new ReportColumn() { Title = "Column A" });
            _reportModel.AddReportColumn(new ReportColumn() { Title = "Column B" });

            Assert.That(_reportModel.ReportColumns.Count(), Is.EqualTo(2));

            Assert.That(_reportModel.ReportColumns.First().Title, Is.EqualTo("Column A"));
            Assert.That(_reportModel.ReportColumns.Last().Title, Is.EqualTo("Column B"));
        }

        [Test]
        public void GetReportColumn_GivenReportColumnTitle_ReportColumnReturned()
        {
            _reportModel.AddReportColumn(new ReportColumn() { Title = "Column A" });
            _reportModel.AddReportColumn(new ReportColumn() { Title = "Column B" });

            var reportColumn = _reportModel.GetReportColumn("Column A");
            
            Assert.NotNull(reportColumn);
            Assert.That(reportColumn.Title, Is.EqualTo("Column A"));
        }

        [Test]
        public void GetReportColumn_GivenUnknownReportColumnTitle_NullReturned()
        {
            _reportModel.AddReportColumn(new ReportColumn() { Title = "Column A" });
            _reportModel.AddReportColumn(new ReportColumn() { Title = "Column B" });

            var reportColumn = _reportModel.GetReportColumn("Column C");

            Assert.Null(reportColumn);
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
                  _reportModel.Get(Enumerable.Empty<IReportColumn>(), null);
              });
        }

        [Test]
        public void Validate_GivenEmptyReportColumnsColection_ThrownReportException()
        {
            _reportModel.AddReportColumn(new ReportColumn()
            {
                Title = "Column A",
                SqlValueExpression = "select A"
            });
            _reportModel.SetDataSource(new ReportDataSource("select * from ..."));


            var columns = Enumerable.Empty<IReportColumn>();
            var filters = Enumerable.Empty<IReportFilter>();

            const string expectedError = "Report must have at least one output column.";
            
            var errors = _reportModel.Validate(columns, filters);

            Assert.That(errors.Length, Is.EqualTo(1));
            Assert.That(errors[0], Is.EqualTo(expectedError));

            var exception = Assert.Throws(typeof(ReportException),
              () =>
              {
                  _reportModel.Get(columns, filters);
              });

            Assert.True(exception.Message.Contains(expectedError));
        }

        [Test]
        public void Validate_GivenUnknownReportColumn_ThrownReportException()
        {
            //Set collection of available for report columns
            _reportModel.AddReportColumn(new ReportColumn()
            {
                Title = "Column A",
                SqlValueExpression = "select A"
            });
            _reportModel.SetDataSource(new ReportDataSource("select * from ..."));

            var columns = new List<IReportColumn>()
            {
                new ReportColumn()
                {
                    Title = "Unknown Column",
                    SqlValueExpression = "select B"
                }
            };
            var filters = Enumerable.Empty<IReportFilter>();

            const string expectedError = "Unknow report column: Unknown Column";

            var errors = _reportModel.Validate(columns, filters);

            Assert.That(errors.Length, Is.EqualTo(1));
            Assert.That(errors[0], Is.EqualTo(expectedError));

            var exception = Assert.Throws(typeof(ReportException),
              () =>
              {
                  _reportModel.Get(columns, filters);
              });

            Assert.True(exception.Message.Contains(expectedError));
        }

        [Test]
        public void Validate_GivenUnknownReportFilter_ThrownReportException()
        {
            //Set collection of available for report columns
            _reportModel.AddReportColumn(new ReportColumn()
            {
                Title = "Column A",
                SqlValueExpression = "select A"
            });
            _reportModel.SetDataSource(new ReportDataSource("select * from ..."));


            var columns = new List<IReportColumn>()
            {
                new ReportColumn()
                {
                    Title = "Column A",
                    SqlValueExpression = "select A"
                }
            };
            var filters = new List<IReportFilter>()
            {
                new ReportFilter()
                {
                    ReportColumn = new ReportColumn()
                    {
                        Title = "Unknown Column",
                        SqlValueExpression = "select B"
                    },
                    Type = FilterType.Equal,
                    Value = "Some value"
                }
            };

            const string expectedError = "Unknow report filter, column: Unknown Column";

            var errors = _reportModel.Validate(columns, filters);

            Assert.That(errors.Length, Is.EqualTo(1));
            Assert.That(errors[0], Is.EqualTo(expectedError));

            var exception = Assert.Throws(typeof(ReportException),
              () =>
              {
                  _reportModel.Get(columns, filters);
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
            var columns = new List<IReportColumn>()
            {
                new ReportColumn()
                {
                    Title = "Column A",
                    SqlValueExpression = "select A"
                }
            };
            var filters = Enumerable.Empty<IReportFilter>();
            string dataSource = "select * from ...";

            var sqlCommand = new SqlCommand("select ColumnA from ....");
            DataTable reportTable = new DataTable();
            reportTable.Columns.Add("ColumnA");
            reportTable.Rows.Add("Row #1");
            reportTable.Rows.Add("Row #2");

            var queryBuilder = new Mock<IQueryBuilder>();
            queryBuilder.Setup(x => x.BuildQuery(columns, filters, dataSource)).Returns(sqlCommand);

            var queryExecutor = new Mock<IQueryExecutor>();
            queryExecutor.Setup(x => x.ExecuteToDataTable(sqlCommand)).Returns(reportTable);

            var reportModel = new ReportModel(queryBuilder.Object, queryExecutor.Object);
            reportModel.AddReportColumn(columns.First());
            reportModel.SetDataSource(new ReportDataSource("select * from ..."));

            var result = reportModel.Get(columns, filters);

            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(result[0].Count, Is.EqualTo(1));
            Assert.True(result[0].ContainsKey("ColumnA"));
            Assert.That(result[0]["ColumnA"], Is.EqualTo("Row #1"));

            Assert.That(result[1].Count, Is.EqualTo(1));
            Assert.True(result[1].ContainsKey("ColumnA"));
            Assert.That(result[1]["ColumnA"], Is.EqualTo("Row #2"));
        }
    }
}
