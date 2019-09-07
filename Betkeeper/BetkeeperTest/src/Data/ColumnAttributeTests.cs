using System;
using System.Collections.Generic;
using Betkeeper.Data;
using NUnit.Framework;

namespace BetkeeperTest.Data
{
    [TestFixture]
    public class ColumnAttributeTests
    {

        [TestCase]
        public void ColumnAttribute_InvalidDataType_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new ColumnAttribute(dataType: "Not valid", columnName: "test");
            });
        }

        [TestCase]
        public void ColumnAttribute_ValidDataType_PropertiesAreSet()
        {
            var columnAttribute = new ColumnAttribute(dataType: "TEXT", columnName: "test");

            Assert.AreEqual(columnAttribute.DataType, "TEXT");
            Assert.AreEqual(columnAttribute.ColumnName, "test");
        }
    }
}
