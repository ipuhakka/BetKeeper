using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

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

    [Serializable]
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
        /// Clones a component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toClone"></param>
        /// <returns></returns>
        public static T CloneComponent<T>(T toClone)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, toClone);

                stream.Seek(0, SeekOrigin.Begin);

                return (T)formatter.Deserialize(stream);
            }
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

            return Component
                .ParseComponent(asJObject.ToString()) as T;
        }

        /// <summary>
        /// Deletes first component from component listing with matching key.
        /// </summary>
        /// <param name="components"></param>
        /// <param name="componentKey"></param>
        public static void DeleteComponent(List<Component> components, string componentKey)
        {
            var match = components.FirstOrDefault(component => component.ComponentKey == componentKey);

            if (match != null)
            {
                components.Remove(match);
                return;
            }

            foreach (var component in components)
            {
                var asContainer = component as Container;
                if (asContainer != null && asContainer.Children.Count > 0)
                {
                    DeleteComponent(asContainer.Children, componentKey);
                }
            }
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

            switch (componentType)
            {
                case "Button":
                    return Button.Parse(asJObject);

                case "Field":
                    return Field.Parse(asJObject);

                case "Table":
                    return Table.Parse(asJObject);

                case "Tab":
                    return Tab.Parse(asJObject);

                case "Container":
                    return Container.Parse(asJObject);

                default:
                    throw new NotImplementedException(
                        $"Component type {componentType} parsing not implemented"
                    );
            }
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
