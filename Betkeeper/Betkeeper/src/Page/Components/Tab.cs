using System;
using System.Collections.Generic;
using System.Text;

namespace Betkeeper.Page.Components
{
    public class Tab : Component
    {
        public string Title { get; }

        public string Key { get; }

        public List<Component> TabContent { get; }

        public Tab(string key, string title, List<Component> tabContent)
            : base(ComponentType.Tab)
        {
            TabContent = tabContent;
            Key = key;
            Title = title;
        }
    }
}
