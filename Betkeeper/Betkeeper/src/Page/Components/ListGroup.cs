using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Betkeeper.Page.Components
{
    public enum ListGroupMode
    {
        /// <summary>
        /// Allow selecting items in list group
        /// and store selected item identifier in data for actions
        /// </summary>
        Selectable,

        /// <summary>
        /// Expand list group item on click
        /// </summary>
        //Expandable,

        /// <summary>
        /// Only display
        /// </summary>
        DisplayOnly
    }

    /// <summary>
    /// List group component class. 
    /// </summary>
    public class ListGroup<T> : Component
    {
        public List<T> Data { get; }

        /// <summary>
        /// Key which is used as item identifier
        /// </summary>
        public string KeyField { get; }

        public List<string> HeaderKeys { get; set; }

        /// <summary>
        /// Data items which value is to be shown as small text below header text
        /// </summary>
        public List<string> SmallItemKeys { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ListGroupMode Mode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="keyField"></param>
        /// <param name="headerKeys"></param>
        /// <param name="smallItemKeys"></param>
        public ListGroup(
            ListGroupMode mode,
            List<T> data, 
            string keyField,
            List<string> headerKeys, 
            List<string> smallItemKeys = null,
            string componentKey = null) : base(ComponentType.ListGroup, componentKey)
        {
            Data = data;
            KeyField = keyField;
            HeaderKeys = headerKeys;
            SmallItemKeys = smallItemKeys;
            Mode = mode;
        }
    }
}
