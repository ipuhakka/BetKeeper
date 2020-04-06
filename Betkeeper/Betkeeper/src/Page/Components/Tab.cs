using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Betkeeper.Page.Components
{
    public class Tab : Component
    {
        public string Title { get; }

        public List<Component> TabContent { get; set; }

        public Tab(string key, string title, List<Component> tabContent)
            : base(ComponentType.Tab, key)
        {
            TabContent = tabContent;
            Title = title;
        }

        public static Tab Parse(JObject asJObject)
        {
            var tabContent = ComponentParser.ParseComponents(asJObject["tabContent"].ToString());
             
            var tab = asJObject.ToObject<Tab>();

            tab.TabContent = tabContent;

            return tab;
        }
    }
}
