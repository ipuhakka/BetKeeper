using Betkeeper.Page.Components;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Betkeeper.Page
{
    /// <summary>
    /// Generic type-based component actions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComponentTools
    {
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

            return ComponentParser
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
    }
}
