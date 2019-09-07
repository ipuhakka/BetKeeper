using System;
using System.Collections.Generic;
using System.Linq;

namespace Betkeeper.Data
{
    public class ColumnAttribute : Attribute
    {

        public string DataType { get; }

        public string ColumnName { get; }

        public ColumnAttribute(string dataType, string columnName)
        {
            if (!IsValidDataType(dataType))
            {
                throw new ArgumentException(
                    string.Format("{0} is not a supported data type", dataType));
            }

            DataType = dataType;
            ColumnName = columnName;
        }

        private bool IsValidDataType(string dataType)
        {
            return new List<string>() { "TEXT", "INTEGER", "REAL" }.Contains(dataType);
        }
    }
}
