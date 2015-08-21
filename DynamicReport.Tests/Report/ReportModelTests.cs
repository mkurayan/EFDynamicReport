using System;
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
    }
}
