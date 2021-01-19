using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Betkeeper.Page
{
    public enum DataType
    {
        DateTime,
        Double,
        Integer,
        String
    }

    public class ItemField
    {
        public string FieldKey { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DataType FieldType { get; }

        public string FieldLegend { get; }

        public ItemField(string fieldKey, DataType fieldType, string fieldLegend = null)
        {
            FieldKey = fieldKey;
            FieldType = fieldType;
            FieldLegend = fieldLegend;
        }
    }
}
