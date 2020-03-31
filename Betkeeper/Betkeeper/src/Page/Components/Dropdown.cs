﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Betkeeper.Page.Components
{
    public class Dropdown : Field
    {
        public List<Option> Options { get; }

        /// <summary>
        /// Call UpdateOptions on changing dropdown value. Allows to bind dropdowns.
        /// </summary>
        public bool UpdateOptionsOnChange { get; }

        public Dropdown(
            string key, 
            string label, 
            List<Option> options, 
            bool updateOptionsOnChange = false)
            : base(key, label, FieldType.Dropdown)
        {
            Options = options;
            UpdateOptionsOnChange = updateOptionsOnChange;
        }

        [JsonConstructor]
        public Dropdown(
            string key,
            string label,
            List<Option> options,
            string dataKey,
            bool readOnly = false,
            bool updateOptionsOnChange = false)
            : base(key, label, readOnly, FieldType.Dropdown, dataKey)
        {
            UpdateOptionsOnChange = updateOptionsOnChange;
            Options = options;
        }
    }

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
