using System.Collections.Generic;

namespace Betkeeper.Page.Components
{
    public class Chart<T> : Component
    {
        /// <summary>
        /// Data item listing
        /// </summary>
        public List<T> Data { get; }

        public ItemField KeyField { get; }

        public List<ItemField> DataFields { get; set; }

        public Chart(
            string componentKey, 
            List<T> data, 
            ItemField keyField,
            List<ItemField> dataFields)
            :base(ComponentType.Chart, componentKey)
        {
            Data = data;
            KeyField = keyField;
            DataFields = dataFields;
        }
    }
}
