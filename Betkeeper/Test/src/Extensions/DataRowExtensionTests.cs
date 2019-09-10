using System;
using System.Data;
using Betkeeper.Extensions;
using Betkeeper;
using NUnit.Framework;

namespace Test.Extensions
{
    [TestFixture]
    public class DataRowExtensionTests
    {
        DataTable TestDataTable;
        DataRow TestDataRow;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestDataTable = new DataTable();
            TestDataTable.Columns.Add("test");
        }

        [SetUp]
        public void SetUp()
        {
            TestDataRow = TestDataTable.NewRow();
        }

        [TearDown]
        public void TearDown()
        {
            TestDataTable.Clear();
        }

        [Test]
        public void ToBetResult_OutOfRange_ReturnsException()
        {

            TestDataRow["test"] = 2;
            TestDataTable.Rows.Add(TestDataRow);

            Assert.Throws<Exception>(() =>
                TestDataRow.ToBetResult("test"));
        }

        [Test]
        public void ToBetResult_ConversionSucceeds()
        {
            TestDataRow["test"] = 1;
            TestDataTable.Rows.Add(TestDataRow);

            Assert.AreEqual(Enums.BetResult.Won, TestDataRow.ToBetResult("test"));
        }

        [Test]
        public void ToDouble_ParsesWithCommaDecimalSeparator()
        {
            TestDataRow["test"] = "1,2";
            TestDataTable.Rows.Add(TestDataRow);

            Assert.AreEqual(1.2, TestDataRow.ToDouble("test"));
        }

        [Test]
        public void ToDouble_ParsesWithDotDecimalSeparator()
        {
            TestDataRow["test"] = "1.2";
            TestDataTable.Rows.Add(TestDataRow);

            Assert.AreEqual(1.2, TestDataRow.ToDouble("test"));
        }

        [Test]
        public void ToDouble_NotValid_ThrowsFormatException()
        {
            TestDataRow["test"] = "notValid.1";
            TestDataTable.Rows.Add(TestDataRow);

            Assert.Throws<FormatException>(() =>
                TestDataRow.ToDouble("test"));
        }
    }
}
