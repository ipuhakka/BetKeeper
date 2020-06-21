﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Betkeeper.Page
{
    public enum DataType
    {
        DateTime,
        Double,
        Integer,
        String
    }

    public class DataField
    {
        public string Key { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DataType DataType { get; }

        public DataField(string key, DataType dataType)
        {
            Key = key;
            DataType = dataType;
        }
    }
}
