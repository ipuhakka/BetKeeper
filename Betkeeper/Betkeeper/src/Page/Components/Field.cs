using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

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

    [Serializable]
    public class Field : Component
    {
        public string DataKey { get; }

        public string Label { get; }

        public bool ReadOnly { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FieldType FieldType { get; }

        public Field(string componentKey, string label, FieldType fieldType)
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

    [Serializable]
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

    [Serializable]
    public class Dropdown : Field
    {
        public List<Option> Options { get; }

        /// <summary>
        /// Keys for components to update on value change.
        /// </summary>
        public List<string> ComponentsToUpdate { get; set; }

        public Dropdown(
            string componentKey,
            string label,
            List<Option> options,
            List<string> componentsToUpdate = null)
            : base(componentKey, label, FieldType.Dropdown)
        {
            Options = options;
            ComponentsToUpdate = componentsToUpdate;
        }

        /// <summary>
        /// Constructor for making options list using available options.
        /// </summary>
        /// <param name="componentKey"></param>
        /// <param name="label"></param>
        /// <param name="options"></param>
        /// <param name="componentsToUpdate"></param>
        public Dropdown(
            string componentKey,
            string label,
            List<string> options,
            List<string> componentsToUpdate = null)
            : base(componentKey, label, FieldType.Dropdown)
        {
            Options = options
                .Select(option => new Option(option, option))
                .ToList();
            ComponentsToUpdate = componentsToUpdate;
        }

        [JsonConstructor]
        public Dropdown(
            string componentKey,
            string label,
            List<Option> options,
            string dataKey,
            bool readOnly = false,
            List<string> componentsToUpdate = null)
            : base(componentKey, label, readOnly, FieldType.Dropdown, dataKey)
        {
            ComponentsToUpdate = componentsToUpdate;
            Options = options;
        }
    }

    /// <summary>
    /// Class for dropdown which is used to input selections
    /// </summary>
    [Serializable]
    public class InputDropdown : Field
    {
        public InputDropdown(string componentKey, string label, string dataKey = null)
            : base(componentKey, label, FieldType.InputDropdown, dataKey: dataKey)
        {

        }
    }

    /// <summary>
    /// Dropdown list option class.
    /// </summary>
    [Serializable]
    public class Option
    {
        public string Key { get; }

        public string Value { get; }

        public bool InitialValue { get; set; }

        public Option(string key, string value, bool initialValue = false)
        {
            Key = key;
            Value = value;
            InitialValue = initialValue;
        }
    }
}
