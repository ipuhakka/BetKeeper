﻿using System;
using System.Collections.Generic;
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

        public Field(string componentKey, string label, FieldType fieldType)
            : base(ComponentType.Field, componentKey)
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
            string componentKey, 
            string label, 
            bool readOnly, 
            FieldType fieldType,
            string dataKey)
            : this(componentKey, label, fieldType)
        {
            ReadOnly = readOnly;
            DataKey = dataKey;
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

    public class Dropdown : Field
    {
        public List<Option> Options { get; }

        /// <summary>
        /// Call UpdateOptions on changing dropdown value. Allows to bind dropdowns.
        /// </summary>
        public bool UpdateOptionsOnChange { get; }

        public Dropdown(
            string componentKey,
            string label,
            List<Option> options,
            bool updateOptionsOnChange = false)
            : base(componentKey, label, FieldType.Dropdown)
        {
            Options = options;
            UpdateOptionsOnChange = updateOptionsOnChange;
        }

        [JsonConstructor]
        public Dropdown(
            string componentKey,
            string label,
            List<Option> options,
            string dataKey,
            bool readOnly = false,
            bool updateOptionsOnChange = false)
            : base(componentKey, label, readOnly, FieldType.Dropdown, dataKey)
        {
            UpdateOptionsOnChange = updateOptionsOnChange;
            Options = options;
        }
    }

    /// <summary>
    /// Dropdown list option class.
    /// </summary>
    public class Option
    {
        public string Key { get; }

        public string Value { get; }

        public Option(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
