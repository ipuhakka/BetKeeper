using System;
using System.Collections.Generic;

namespace Betkeeper.Page.Components
{
    public class Dropdown : Field
    {
        public List<Option> Options { get; }

        public Dropdown(string key, string label, List<Option> options)
            : base(key, label, FieldType.Dropdown)
        {
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
