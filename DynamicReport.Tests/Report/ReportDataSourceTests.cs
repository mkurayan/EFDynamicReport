using System;
using DynamicReport.Report;
using NUnit.Framework;

namespace DynamicReport.Tests.Report
{
    [TestFixture]
    class ReportDataSourceTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ctor_GivenEmptyDataSource_ThrownArgumentNullException(string query)
        {
            Assert.Throws(typeof(ArgumentNullException),
              () =>
              {
                  new ReportDataSource(query);
              });
        }
    }
}
