using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Betkeeper.Page.Components
{
    [Serializable]
    public class Tab : Component
    {
        public string Title { get; }

        public List<Component> TabContent { get; set; }

        public Tab(string componentKey, string title, List<Component> tabContent)
            : base(ComponentType.Tab, componentKey)
        {
            TabContent = tabContent;
            Title = title;
        }

        public static Tab Parse(JObject asJObject)
        {
            var tabContent = Component.ParseComponents(asJObject["tabContent"].ToString());

            var tab = asJObject.ToObject<Tab>();

            tab.TabContent = tabContent;

            return tab;
        }
    }
}
