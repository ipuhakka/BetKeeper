using System.Collections.Generic;

namespace Betkeeper.Page.Components
{
    public class Table : Component
    {
        public string DataKey { get; }

        /// <summary>
        /// Keys not shown in table.
        /// </summary>
        public List<string> HiddenKeys { get; }

        public Table(string dataKey, List<string> hiddenKeys = null)
            : base(ComponentType.Table)
        {
            DataKey = dataKey;
            HiddenKeys = hiddenKeys;
        }
    }
}
