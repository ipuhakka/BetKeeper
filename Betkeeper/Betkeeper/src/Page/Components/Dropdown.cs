using System;
using System.Collections.Generic;

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
