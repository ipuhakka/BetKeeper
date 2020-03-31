using System;
using System.Collections.Generic;
using System.Text;

namespace Betkeeper.Page.Components
{
    public class Tab : Component
    {
        public string Title { get; }

        public List<Component> TabContent { get; }

        public Tab(string key, string title, List<Component> tabContent)
            : base(ComponentType.Tab, key)
        {
            TabContent = tabContent;
            Title = title;
        }
    }
}
