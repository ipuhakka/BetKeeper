using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Betkeeper.Page.Components
{
    [Serializable]
    public class Table : Component
    {
        public string DataKey { get; }

        /// <summary>
        /// Columns shown in table.
        /// </summary>
        public List<ItemField> Columns { get; }

        /// <summary>
        /// Path to which user is navigated on clicking row.
        /// </summary>
        public string NavigationPath { get;  }

        /// <summary>
        /// Data key which is used in url to open a row.
        /// </summary>
        public string NavigationKey { get; }

        public Table(
            string dataKey,
            List<ItemField> columns,
            string navigationPath = null,
            string navigationKey = null)
            : base(ComponentType.Table)
        {
            DataKey = dataKey;
            Columns = columns;
            NavigationPath = navigationPath;
            NavigationKey = navigationKey;
        }

        public static Table Parse(JObject asJObject)
        {
            return asJObject.ToObject<Table>();
        }
    }
}
