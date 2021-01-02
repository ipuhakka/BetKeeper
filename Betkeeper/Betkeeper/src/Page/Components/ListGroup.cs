using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System;

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
        Expandable,

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

        /// <summary>
        /// Data items shown on header
        /// </summary>
        public List<ItemField> HeaderItems { get; set; }

        /// <summary>
        /// Data items which value is to be shown as small text below header text
        /// </summary>
        public List<ItemField> SmallItems { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ListGroupMode Mode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="data"></param>
        /// <param name="keyField"></param>
        /// <param name="headerItems"></param>
        /// <param name="smallItems"></param>
        /// <param name="componentKey"></param>
        public ListGroup(
            ListGroupMode mode,
            List<T> data, 
            string keyField,
            List<ItemField> headerItems, 
            List<ItemField> smallItems = null,
            string componentKey = null) : base(ComponentType.ListGroup, componentKey)
        {
            Data = data;
            KeyField = keyField;
            HeaderItems = headerItems;
            SmallItems = smallItems;
            Mode = mode;
        }
    }

    /// <summary>
    /// Class for list gorup which expands items on click.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExpandableListGroup<T> : ListGroup<T>
    {
        /// <summary>
        /// Item specific actions
        /// </summary>
        public List<Button> ItemActions { get; set; }

        /// <summary>
        /// Fields shown in expanded list group
        /// </summary>
        public List<Field> ItemFields { get; set; }

        public ExpandableListGroup(
            List<T> data,
            string keyField,
            List<ItemField> headerItems,
            List<ItemField> smallItems = null,
            string componentKey = null)
            : base(ListGroupMode.Expandable, data, keyField, headerItems, smallItems, componentKey)
        {
        }
    }

    public class ItemContent
    {
        public List<Button> ItemActions { get; }

        public List<Field> ItemFields { get; }

        public object Data { get; }

        public ItemContent(List<Field> itemFields, List<Button> itemActions, object data)
        {
            ItemFields = itemFields;
            ItemActions = itemActions;
            Data = data;
        }
    }
}
