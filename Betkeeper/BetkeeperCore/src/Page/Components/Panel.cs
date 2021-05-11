using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Betkeeper.Page.Components
{
    /// <summary>
    /// Component for a closable panel that does not affect datapath of elements
    /// </summary>
    public class Panel : Component
    {
        public List<Component> Children { get; set; }

        public string Legend { get; set; }

        public Panel(List<Component> children, string componentKey, string legend = null) : base(ComponentType.Panel, componentKey)
        {
            Legend = legend ?? componentKey;
            Children = children;
        }

        public static Panel Parse(JObject asJObject)
        {
            var panel = asJObject.ToObject<Panel>();
            panel.Children = ParseComponents(asJObject["children"].ToString());
            return panel;
        }
    }
}
