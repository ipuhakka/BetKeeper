using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Betkeeper.Page.Components
{
    public enum InputType
    {
        TextField,
        TextBox,
        Int,
        Double,
        DateTime
    }

    public class Input : Component
    {
        public string Key { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public InputType InputType { get; }

        public Input(string key, InputType inputType)
            : base(ComponentType.Input)
        {
            Key = key;
            InputType = inputType;
        }
    }
}
