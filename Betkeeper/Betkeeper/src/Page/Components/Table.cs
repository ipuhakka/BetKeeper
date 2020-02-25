using System.Collections.Generic;

namespace Betkeeper.Page.Components
{
    public class Table : Component
    {
        public string DataKey { get; }

        public Table(string dataKey)
            : base(ComponentType.Table)
        {
            DataKey = dataKey;
        }
    }
}
