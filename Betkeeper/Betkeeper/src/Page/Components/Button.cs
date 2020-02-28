using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Betkeeper.Page.Components
{
    public enum ButtonType
    {
        PageAction,
        ModalAction,
        Navigation
    }

    public class Button : Component
    {
        public string Text { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ButtonType ButtonType { get; }

        public string Style { get; }

        public bool RequireConfirm { get; }

        public Button (
            ButtonType buttonType, 
            string text, 
            string style = "primary", 
            bool requireConfirm = false)
            : base(ComponentType.Button)
        {
            Text = text;
            ButtonType = buttonType;
            Style = style;
            RequireConfirm = requireConfirm;
        }
    }

    public class NavigationButton : Button
    {
        public string NavigateTo { get; }

        public NavigationButton(string navigateTo, string text, string style) 
            : base(ButtonType.Navigation, text, style)
        {
            NavigateTo = navigateTo;
        }
    }

    public class PageActionButton : Button
    {
        public string Action { get; }

        /// <summary>
        /// Keys for data to be sent in action request.
        /// </summary>
        public List<string> ActionDataKeys { get; }

        public PageActionButton(
            string action, 
            List<string> actionDataKeys, 
            string text, 
            string style = "outline-primary",
            bool requireConfirm = false)
            : base(ButtonType.PageAction, text, style, requireConfirm)
        {
            Action = action;
            ActionDataKeys = actionDataKeys;
        }
    }


    /// <summary>
    /// Button which opens a modal for performing action
    /// </summary>
    public class ModalActionButton : Button
    {
        public string Action { get; }

        /// <summary>
        /// Modal fields.
        /// </summary>
        public List<Field> ModalFields { get; }

        public ModalActionButton(
            string action, 
            List<Field> fields,
            string text,
            string style = "primary",
            bool requireConfirm = false)
            : base(ButtonType.ModalAction, text, style, requireConfirm)
        {
            Action = action;
            ModalFields = fields;
        }
    }
}
