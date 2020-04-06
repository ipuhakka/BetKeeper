using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Betkeeper.Page.Components;

namespace Betkeeper.Page
{
    public class ComponentParser
    {

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
