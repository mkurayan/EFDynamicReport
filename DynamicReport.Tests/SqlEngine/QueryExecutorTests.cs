using System;
using DynamicReport.SqlEngine;
using NUnit.Framework;

namespace DynamicReport.Tests.SqlEngine
{
    [TestFixture]
    class QueryExecutorTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ctor_ConnectionStringNotProvided_ArgumentExceptionThrown(string connectionString)
        {
            Assert.Throws(typeof (ArgumentException),
                () => new QueryExecutor(connectionString));
        }
    }
}
