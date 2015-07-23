using DynamicReport.Report;
using NUnit.Framework;

namespace DynamicReport.Tests.Report
{
    [TestFixture]
    class ReportFieldTests
    {
        [Test]
        public void SqlAlias_GivenSingleWordTitle_ReturnFormattedValue()
        {
            ReportField reportField = new ReportField()
            {
                Title = "Something"
            };

            Assert.That(reportField.SqlAlias, Is.EqualTo("Something"));
        }

        [Test]
        public void SqlAlias_GivenMultipleWordsTitle_ReturnFormattedValue()
        {
            ReportField reportField = new ReportField()
            {
                Title = "Field Title"
            };

            Assert.That(reportField.SqlAlias, Is.EqualTo("FieldTitle"));
        }
    }
}
