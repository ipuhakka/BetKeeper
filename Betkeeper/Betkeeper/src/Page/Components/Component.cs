using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Betkeeper.Page.Components
{
    public enum ComponentType
    {
        Button,
        Input,
    }

    public class Component
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ComponentType ComponentType { get; }

        public Component(ComponentType componentType)
        {
            ComponentType = componentType;
        }
    }
}
