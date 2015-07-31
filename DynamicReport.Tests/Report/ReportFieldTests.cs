using DynamicReport.Report;
using NUnit.Framework;

namespace DynamicReport.Tests.Report
{
    [TestFixture]
    class ReportFieldTests
    {
        private ReportField originalField;
        private ReportField copyOfOriginalField;
        private ReportField differentField; 

        [SetUp]
        public void SetUp()
        {
            originalField = new ReportField
            {
                Title = "First Field",
                SqlValueExpression = "TestTable.fField",
            };

            copyOfOriginalField = new ReportField
            {
                Title = "First Field",
                SqlValueExpression = "TestTable.fField",
            };

            differentField = new ReportField
            {
                Title = "Second Field",
                SqlValueExpression = "TestTable.sField",
            };
        }  

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

        [Test]
        public void Equals_GivenSingeleField_FieldEqualsToItself()
        {
            Assert.IsTrue(originalField.Equals(originalField));
            Assert.IsTrue(originalField.Equals((object)originalField));

            Assert.IsTrue(originalField == originalField);
        }

        [Test]
        public void Equals_GivenTwoEqualsObjects_ObjectsAreEquals()
        {
            Assert.IsTrue(originalField.Equals(copyOfOriginalField));
            Assert.IsTrue(originalField.Equals((object)copyOfOriginalField));

            Assert.IsTrue(copyOfOriginalField.Equals(originalField));
            Assert.IsTrue(copyOfOriginalField.Equals((object)originalField));
            Assert.IsTrue(originalField == copyOfOriginalField);
            Assert.IsTrue(copyOfOriginalField == originalField);
        }

        [Test]
        public void Equals_GivenTwoNotEqualsObjects_EqualsMethodReturnFalse()
        {
            ReportField a = null;
            ReportField b = null;

            Assert.IsTrue(a == b);

            //compare with null
            Assert.IsFalse(originalField.Equals(null));
            Assert.IsFalse(originalField == null);
            Assert.IsFalse(null == originalField);

            //compare objects with different types.
            Assert.IsFalse(originalField.Equals(new object()));
            Assert.IsFalse(originalField.Equals("String object"));
            Assert.IsFalse(originalField.Equals(42));

            //compare objects with same type 
            Assert.IsFalse(originalField.Equals(differentField));
            Assert.IsFalse(originalField.Equals((object)differentField));
            Assert.IsFalse(originalField == differentField);
            Assert.IsTrue(originalField != differentField);

            //compare objects with different titles
            a = new ReportField()
            {
                Title = "FieldA",
                SqlValueExpression = "Exp"
            };

            b = new ReportField()
            {
                Title = "FieldB",
                SqlValueExpression = "Exp"
            };

            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(b.Equals(a));
            Assert.IsFalse(a == b);
            Assert.IsFalse(b == a);

            //compare objects with different SqlValueExpressions
            a = new ReportField()
            {
                Title = "Field",
                SqlValueExpression = "ExpA"
            };

            b = new ReportField()
            {
                Title = "Field",
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
            var a = new ReportField()
            {
                Title = "FieldA",
                SqlValueExpression = "Exp"
            };

            var b = new ReportField()
            {
                Title = "FieldA",
                SqlValueExpression = "Exp"
            };

            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_GivenTwoNotEqualsObject_MethodReturDifferentHashCodes()
        {
            var a = new ReportField()
            {
                Title = "FieldA", // <---
                SqlValueExpression = "Exp"
            };

            var b = new ReportField()
            {
                Title = "FieldB", // <---
                SqlValueExpression = "Exp"
            };

            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));

            a = new ReportField()
            {
                Title = "FieldA",
                SqlValueExpression = "ExpA" // <---
            };

            b = new ReportField()
            {
                Title = "FieldA",
                SqlValueExpression = "ExpB" // <---
            };

            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }
    }
}
