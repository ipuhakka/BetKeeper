using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Betkeeper.Page.Components
{
    public enum FieldType
    {
        DateTime,
        Integer,
        Double,
        TextBox,
        TextArea
    }

    public class Field
    {
        public string Key { get; }

        public string Label { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FieldType FieldType { get; }

        public Field(string key, string label, FieldType fieldType)
        {
            Key = key;
            Label = label;
            FieldType = fieldType;
        }
    }
}
