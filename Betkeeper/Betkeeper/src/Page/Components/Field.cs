using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace Betkeeper.Page.Components
{
    public enum FieldType
    {
        DateTime,
        Integer,
        Double,
        TextBox,
        TextArea,
        Dropdown
    }

    public class Field : Component
    {
        public string DataKey { get; }

        public string Label { get; }

        public bool ReadOnly { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FieldType FieldType { get; }

        public Field(string key, string label, FieldType fieldType)
            : base(ComponentType.Field, key)
        {
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

        [JsonConstructor]
        public Field(
            string key,
            string label,
            FieldType fieldType,
            bool readOnly = false,
            string dataKey = null)
            : this(key, label, readOnly, fieldType, dataKey)
        {
        }

        /// <summary>
        /// Parses a jObject into a field
        /// </summary>
        /// <param name="asJObject"></param>
        /// <returns></returns>
        public static Field Parse(JObject asJObject)
        {
            var fieldType = asJObject["FieldType"].ToString();

            switch (fieldType)
            {
                case "Dropdown":
                    return asJObject.ToObject<Dropdown>();

                default:
                    return asJObject.ToObject<Field>();
            }
        }
    }
}
