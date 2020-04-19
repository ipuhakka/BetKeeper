using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Betkeeper.Page.Components
{
    [Serializable]
    public class Table : Component
    {
        public string DataKey { get; }

        /// <summary>
        /// Columns shown in table.
        /// </summary>
        public List<DataField> Columns { get; }

        /// <summary>
        /// Data key which is used in url to open a row.
        /// </summary>
        public string NavigationKey { get; }

        public Table(
            string dataKey, 
            List<DataField> columns,
            string navigationKey = null)
            : base(ComponentType.Table)
        {
            DataKey = dataKey;
            Columns = columns;
            NavigationKey = navigationKey;
        }

        public static Table Parse(JObject asJObject)
        {
            return asJObject.ToObject<Table>();
        }
    }
}
