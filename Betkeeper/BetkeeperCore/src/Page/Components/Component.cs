using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

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
        Tab,
        /// <summary>
        /// Label without attached field
        /// </summary>
        Label,
        /// <summary>
        /// Table which data is purely constructed server side
        /// </summary>
        StaticTable,
        ListGroup,
        Chart,

        CardMenu
    }

    public class Component
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ComponentType ComponentType { get; }

        public string ComponentKey { get; set; }

        public string CustomCssClass { get; set; }

        public Component(ComponentType componentType, string componentKey = null)
        {
            ComponentType = componentType;
            ComponentKey = componentKey;
        }

        /// <summary>
        /// Get component from action.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="componentKey"></param>
        /// <returns></returns>
        public static T GetComponentFromAction<T>(
            PageAction action,
            string componentKey) where T : Component
        {
            var asJObject = JObject.Parse(action.Parameters["components"].ToString())[componentKey];

            return ParseComponent(asJObject.ToString()) as T;
        }

        /// <summary>
        /// Parses a component from json.
        /// </summary>
        /// <param name="componentJson"></param>
        /// <returns></returns>
        public static Component ParseComponent(string componentJson)
        {
            var asJObject = JObject.Parse(componentJson);

            var componentType = asJObject["componentType"].ToString();

            return componentType switch
            {
                "Button" => Button.Parse(asJObject),
                "Field" => Field.Parse(asJObject),
                "Table" => Table.Parse(asJObject),
                "Tab" => Tab.Parse(asJObject),
                "Container" => Container.Parse(asJObject),
                _ => throw new NotImplementedException(
                    $"Component type {componentType} parsing not implemented"
),
            };
        }

        /// <summary>
        /// Parses a list of components.
        /// </summary>
        /// <param name="componentsAsJson"></param>
        /// <returns></returns>
        public static List<Component> ParseComponents(string componentsAsJson)
        {
            var asJArray = JArray.Parse(componentsAsJson);

            return asJArray
                .Select(jToken => ParseComponent(jToken.ToString()))
                .ToList();
        }
    }
}
