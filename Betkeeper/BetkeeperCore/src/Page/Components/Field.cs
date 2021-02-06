using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;

namespace Betkeeper.Page.Components
{
    public enum FieldType
    {
        DateTime,
        Integer,
        Double,
        TextBox,
        TextArea,
        Dropdown,
        InputDropdown
    }

    public class Field : Component
    {
        public string DataKey { get; }

        public string Label { get; }

        public bool ReadOnly { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FieldType FieldType { get; }

        public Field(
            string componentKey, 
            string label, 
            FieldType fieldType)
            : base(ComponentType.Field, componentKey)
        {
            Label = label;
            FieldType = fieldType;
            ReadOnly = false;
            DataKey = componentKey;
        }

        /// <summary>
        /// Constructor for data bound field.
        /// </summary>
        /// <param name="componentKey"></param>
        /// <param name="label"></param>
        /// <param name="readOnly"></param>
        /// <param name="fieldType"></param>
        /// <param name="dataKey"></param>
        public Field(
            string componentKey,
            string label,
            bool readOnly,
            FieldType fieldType,
            string dataKey)
            : this(componentKey, label, fieldType)
        {
            ReadOnly = readOnly;
            // Set DataKey either as specified or as componentKey
            DataKey = dataKey ?? componentKey;
        }

        [JsonConstructor]
        public Field(
            string componentKey,
            string label,
            FieldType fieldType,
            bool readOnly = false,
            string dataKey = null)
            : this(componentKey, label, readOnly, fieldType, dataKey)
        {
        }

        /// <summary>
        /// Parses a jObject into a field
        /// </summary>
        /// <param name="asJObject"></param>
        /// <returns></returns>
        public static Field Parse(JObject asJObject)
        {
            var fieldType = asJObject["fieldType"].ToString();

            switch (fieldType)
            {
                case "Dropdown":
                    return asJObject.ToObject<Dropdown>();

                case "DateTime":
                    return asJObject.ToObject<DateTimeInput>();

                default:
                    return asJObject.ToObject<Field>();
            }
        }
    }

    public class HiddenInput : Field
    {
        public bool HideText = true;

        public HiddenInput(string componentKey, string label)
            : base(componentKey, label, FieldType.TextBox)
        {

        }
    }

    public class DateTimeInput : Field
    {
        public DateTime? MinimumDateTime { get; }

        public DateTimeInput(
            string componentKey,
            string label,
            DateTime? minimumDateTime = null,
            bool readOnly = false,
            string dataKey = null)
            : base(componentKey, label, FieldType.DateTime, readOnly: readOnly, dataKey: dataKey)
        {
            MinimumDateTime = minimumDateTime;
        }
    }
}
