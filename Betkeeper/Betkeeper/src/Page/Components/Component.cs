using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Betkeeper.Page.Components
{
    public enum ComponentType
    {
        Button,

        /// <summary>
        /// Field not containing preliminary data
        /// </summary>
        Field,

        Table,

        Container,

        Tab
    }

    public class Component
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ComponentType ComponentType { get; }

        public string Key { get; }

        public Component(ComponentType componentType, string key = null)
        {
            ComponentType = componentType;
            Key = key;
        }
    }
}
