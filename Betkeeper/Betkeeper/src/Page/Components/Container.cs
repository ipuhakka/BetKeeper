using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Betkeeper.Page.Components
{
    /// <summary>
    /// Component for grouping children
    /// </summary>
    public class Container : Component
    {

        public List<Component> Children { get; set; }

        public Container(List<Component> children, string key = null)
            : base(ComponentType.Container, key)
        {
            Children = children;
        }

        public static Container Parse(JObject asJObject)
        {
            var container = asJObject.ToObject<Container>();

            container.Children = ComponentParser.ParseComponents(asJObject["Children"].ToString());

            return container;
        }
    }
}
