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

        /// <summary>
        /// Data key which is used in url to open a row.
        /// </summary>
        public string NavigationKey { get; }

        public Table(
            string dataKey, 
            List<string> hiddenKeys = null,
            string navigationKey = null)
            : base(ComponentType.Table)
        {
            DataKey = dataKey;
            HiddenKeys = hiddenKeys;
            NavigationKey = navigationKey;
        }
    }
}
