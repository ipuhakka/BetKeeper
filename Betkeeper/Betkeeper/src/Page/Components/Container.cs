using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Betkeeper.Page.Components
{
    /// <summary>
    /// Component for grouping children
    /// </summary>
    [Serializable]
    public class Container : Component
    {

        public List<Component> Children { get; set; }

        public Container(List<Component> children, string componentKey = null)
            : base(ComponentType.Container, componentKey)
        {
            Children = children;
        }

        public static Container Parse(JObject asJObject)
        {
            var container = asJObject.ToObject<Container>();

            container.Children = ComponentParser.ParseComponents(asJObject["children"].ToString());

            return container;
        }
    }
}
