using System;
using System.Data;
using Betkeeper.Extensions;
using NUnit.Framework;

namespace Betkeeper.Test.Extensions
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

        [Test]
        public void ToInt32_DecimalWithPoint_ThrowsException()
        {
            TestDataRow["test"] = "1.1";
            TestDataTable.Rows.Add(TestDataRow);

            Assert.Throws<Exception>(() =>
                TestDataRow.ToInt32("test"));  
        }

        [Test]
        public void ToInt32_DecimalWithComma_ThrowsException()
        {
            TestDataRow["test"] = "1,1";
            TestDataTable.Rows.Add(TestDataRow);

            Assert.Throws<Exception>(() =>
                TestDataRow.ToInt32("test"));
        }

        [Test]
        public void ToInt32_ValidIntegerAsString_Succeeds()
        {
            TestDataRow["test"] = "1";
            TestDataTable.Rows.Add(TestDataRow);

            Assert.AreEqual(1, TestDataRow.ToInt32("test"));
        }

        [Test]
        public void ToInt32_ValidInteger_Succeeds()
        {
            TestDataRow["test"] = 1;
            TestDataTable.Rows.Add(TestDataRow);

            Assert.AreEqual(1, TestDataRow.ToInt32("test"));
        }

        [Test]
        public void ToDateTime_yyyy_MM_dd_hh_mm_ss_Succeeds()
        {
            var dateAsString = "2019-01-01 16:45:12";

            TestDataRow["test"] = dateAsString;
            TestDataTable.Rows.Add(TestDataRow);

            var expected = new DateTime(2019, 1, 1, 16, 45, 12);
            Assert.AreEqual(expected, TestDataRow.ToDateTime("test"));
        }
    }
}
