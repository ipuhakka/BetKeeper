using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Betkeeper.Page.Components
{
    /// <summary>
    /// Component for grouping children
    /// </summary>
    [Serializable]
    public class Container : Component
    {

        public List<Component> Children { get; set; }

        /// <summary>
        /// How data is stored client side. 
        /// If true, container child data is stored as an array.
        /// </summary>
        public bool StoreDataAsArray { get; set; }

        public Container(List<Component> children, string componentKey = null, bool storeDataAsArray = false)
            : base(ComponentType.Container, componentKey)
        {
            Children = children;
            StoreDataAsArray = storeDataAsArray;
        }

        /// <summary>
        /// Empties a container.
        /// </summary>
        public void Clear()
        {
            Children = new List<Component>();
        }

        public static Container Parse(JObject asJObject)
        {
            var container = asJObject.ToObject<Container>();

            container.Children = Component.ParseComponents(asJObject["children"].ToString());

            return container;
        }
    }
}
