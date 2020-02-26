using System.Collections.Generic;

namespace Betkeeper.Page.Components
{
    /// <summary>
    /// Component for grouping children
    /// </summary>
    public class Container : Component
    {
        public List<Component> Children { get; }

        public Container(List<Component> children)
            : base(ComponentType.Container)
        {
            Children = children;
        }
    }
}
