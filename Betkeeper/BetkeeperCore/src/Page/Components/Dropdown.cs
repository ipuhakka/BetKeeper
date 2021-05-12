using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Betkeeper.Page.Components
{
    public abstract class DropdownBase : Field
    {
        public List<Option> Options { get; }

        public DropdownBase(
            string componentKey, 
            string label, 
            FieldType fieldType, 
            List<Option> options,
            string dataKey = null,
            bool readOnly = false)
            : base(componentKey, label, readOnly, fieldType, dataKey)
        {
            Options = options;
        }
    }

    public class Dropdown : DropdownBase
    {
        /// <summary>
        /// Allow multiple selection
        /// </summary>
        public bool MultipleSelection { get; set; }

        /// <summary>
        /// If specified for multiple selection dropdown, describes the number of selection allowed
        /// </summary>
        public int? AllowedSelectionCount { get; set; }

        /// <summary>
        /// Keys for components to update on value change.
        /// </summary>
        public List<string> ComponentsToUpdate { get; set; }

        /// <summary>
        /// Dropdown constructor
        /// </summary>
        /// <param name="componentKey"></param>
        /// <param name="label"></param>
        /// <param name="options"></param>
        /// <param name="componentsToUpdate"></param>
        /// <param name="multiple"></param>
        public Dropdown(
            string componentKey,
            string label,
            List<Option> options,
            List<string> componentsToUpdate = null,
            bool multiple = false)
            : base(componentKey, label, FieldType.Dropdown, options)
        {
            ComponentsToUpdate = componentsToUpdate;
            MultipleSelection = multiple;
        }

        /// <summary>
        /// Constructor for making options list using available options.
        /// </summary>
        /// <param name="componentKey"></param>
        /// <param name="label"></param>
        /// <param name="options"></param>
        /// <param name="componentsToUpdate"></param>
        /// <param name="multiple"></param>
        public Dropdown(
            string componentKey,
            string label,
            List<string> options,
            List<string> componentsToUpdate = null,
            bool multiple = false)
            : base(
                  componentKey, 
                  label, 
                  FieldType.Dropdown, 
                  options
                    ?.Select(option => new Option(option, option))
                    .ToList() ?? new List<Option>())
        {
            ComponentsToUpdate = componentsToUpdate;
            MultipleSelection = multiple;
        }

        [JsonConstructor]
        public Dropdown(
            string componentKey,
            string label,
            List<Option> options,
            string dataKey,
            bool readOnly = false,
            List<string> componentsToUpdate = null)
            : base(componentKey, label, FieldType.Dropdown, options, dataKey, readOnly)
        {
            ComponentsToUpdate = componentsToUpdate;
        }
    }

    /// <summary>
    /// Class for dropdown which is used to input selections
    /// </summary>
    public class InputDropdown : Field
    {
        public InputDropdown(string componentKey, string label, string dataKey = null)
            : base(componentKey, label, FieldType.InputDropdown, dataKey: dataKey)
        {

        }
    }

    /// <summary>
    /// Dropdown list option class.
    /// </summary>
    public class Option
    {
        public string Key { get; }

        public string Value { get; }

        public bool InitialValue { get; set; }

        public Option(string key, string value, bool initialValue = false)
        {
            Key = key;
            Value = value;
            InitialValue = initialValue;
        }
    }
}
