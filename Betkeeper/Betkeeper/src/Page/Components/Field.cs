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

    public class Field : Component
    {
        public string Key { get; }

        public string DataKey { get; }

        public string Label { get; }

        public bool ReadOnly { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FieldType FieldType { get; }

        public Field(string key, string label, FieldType fieldType)
            : base(ComponentType.Field)
        {
            Key = key;
            Label = label;
            FieldType = fieldType;
            ReadOnly = false;
        }

        /// <summary>
        /// Constructor for data bound field.
        /// </summary>
        /// <param name="dataKey"></param>
        /// <param name="readOnly"></param>
        /// <param name="fieldType"></param>
        public Field(
            string key, 
            string label, 
            bool readOnly, 
            FieldType fieldType,
            string dataKey)
            : this(key, label, fieldType)
        {
            ReadOnly = readOnly;
            DataKey = dataKey;
        }
    }
}
