using DynamicReport.Report;
using NUnit.Framework;

namespace DynamicReport.Tests.Report
{
    [TestFixture]
    class ReportColumnTests
    {
        private ReportColumn originalColumn;
        private ReportColumn copyOfOriginalColumn;
        private ReportColumn differentColumn; 

        [SetUp]
        public void SetUp()
        {
            originalColumn = new ReportColumn
            {
                Title = "First Column",
                SqlValueExpression = "TestTable.fColumn",
            };

            copyOfOriginalColumn = new ReportColumn
            {
                Title = "First Column",
                SqlValueExpression = "TestTable.fColumn",
            };

            differentColumn = new ReportColumn
            {
                Title = "Second Column",
                SqlValueExpression = "TestTable.sColumn",
            };
        }  

        [Test]
        public void SqlAlias_GivenSingleWordTitle_ReturnFormattedValue()
        {
            ReportColumn reportColumn = new ReportColumn()
            {
                Title = "Something"
            };

            Assert.That(reportColumn.SqlAlias, Is.EqualTo("Something"));
        }

        [Test]
        public void SqlAlias_GivenMultipleWordsTitle_ReturnFormattedValue()
        {
            ReportColumn reportColumn = new ReportColumn()
            {
                Title = "Column Title"
            };

            Assert.That(reportColumn.SqlAlias, Is.EqualTo("ColumnTitle"));
        }

        [Test]
        public void Equals_GivenSingeleColumn_ColumnEqualsToItself()
        {
            Assert.IsTrue(originalColumn.Equals(originalColumn));
            Assert.IsTrue(originalColumn.Equals((object)originalColumn));

            Assert.IsTrue(originalColumn == originalColumn);
        }

        [Test]
        public void Equals_GivenTwoEqualsObjects_ObjectsAreEquals()
        {
            Assert.IsTrue(originalColumn.Equals(copyOfOriginalColumn));
            Assert.IsTrue(originalColumn.Equals((object)copyOfOriginalColumn));

            Assert.IsTrue(copyOfOriginalColumn.Equals(originalColumn));
            Assert.IsTrue(copyOfOriginalColumn.Equals((object)originalColumn));
            Assert.IsTrue(originalColumn == copyOfOriginalColumn);
            Assert.IsTrue(copyOfOriginalColumn == originalColumn);
        }

        [Test]
        public void Equals_GivenTwoNotEqualsObjects_EqualsMethodReturnFalse()
        {
            ReportColumn a = null;
            ReportColumn b = null;

            Assert.IsTrue(a == b);

            //compare with null
            Assert.IsFalse(originalColumn.Equals(null));
            Assert.IsFalse(originalColumn == null);
            Assert.IsFalse(null == originalColumn);

            //compare objects with different types.
            Assert.IsFalse(originalColumn.Equals(new object()));
            Assert.IsFalse(originalColumn.Equals("String object"));
            Assert.IsFalse(originalColumn.Equals(42));

            //compare objects with same type 
            Assert.IsFalse(originalColumn.Equals(differentColumn));
            Assert.IsFalse(originalColumn.Equals((object)differentColumn));
            Assert.IsFalse(originalColumn == differentColumn);
            Assert.IsTrue(originalColumn != differentColumn);

            //compare objects with different titles
            a = new ReportColumn()
            {
                Title = "ColumnA",
                SqlValueExpression = "Exp"
            };

            b = new ReportColumn()
            {
                Title = "ColumnB",
                SqlValueExpression = "Exp"
            };

            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(b.Equals(a));
            Assert.IsFalse(a == b);
            Assert.IsFalse(b == a);

            //compare objects with different SqlValueExpressions
            a = new ReportColumn()
            {
                Title = "Column",
                SqlValueExpression = "ExpA"
            };

            b = new ReportColumn()
            {
                Title = "Column",
                SqlValueExpression = "ExpB"
            };

            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(b.Equals(a));
            Assert.IsFalse(a == b);
            Assert.IsFalse(b == a);
        }

        [Test]
        public void GetHashCode_GivenTwoEqualsObject_MethodReturEqualHashCodes()
        {
            var a = new ReportColumn()
            {
                Title = "ColumnA",
                SqlValueExpression = "Exp"
            };

            var b = new ReportColumn()
            {
                Title = "ColumnA",
                SqlValueExpression = "Exp"
            };

            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_GivenTwoNotEqualsObject_MethodReturDifferentHashCodes()
        {
            var a = new ReportColumn()
            {
                Title = "ColumnA", // <---
                SqlValueExpression = "Exp"
            };

            var b = new ReportColumn()
            {
                Title = "ColumnB", // <---
                SqlValueExpression = "Exp"
            };

            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));

            a = new ReportColumn()
            {
                Title = "ColumnA",
                SqlValueExpression = "ExpA" // <---
            };

            b = new ReportColumn()
            {
                Title = "ColumnA",
                SqlValueExpression = "ExpB" // <---
            };

            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void Equals_Symmetric()
        {
            var x = new ReportColumn()
            {
                Title = "Column A",
                SqlValueExpression = "select 1"
            };
            var y = new ReportColumn()
            {
                Title = "Column A",
                SqlValueExpression = "select 1"
            };
            Assert.IsTrue(x.Equals(y) && y.Equals(x));
            Assert.IsTrue(x.GetHashCode() == y.GetHashCode());
        }
    }
}
