using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace Betkeeper.Page.Components
{
    public enum ButtonType
    {
        PageAction,
        ModalAction,
        Navigation
    }

    public abstract class Button : Component
    {
        public string Text { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ButtonType ButtonType { get; }

        public string Style { get; }

        public bool RequireConfirm { get; }

        public string NavigateTo { get; }

        public Button (
            ButtonType buttonType, 
            string text, 
            string style = "primary", 
            bool requireConfirm = false,
            string navigateTo = null)
            : base(ComponentType.Button)
        {
            Text = text;
            ButtonType = buttonType;
            Style = style;
            RequireConfirm = requireConfirm;
            NavigateTo = navigateTo;
        }

        /// <summary>
        /// Parses a button from jObject
        /// </summary>
        /// <param name="asJObject"></param>
        /// <returns></returns>
        public static Button Parse(JObject asJObject)
        {
            var buttonType = asJObject["buttonType"].ToString();

            switch (buttonType)
            {
                default:
                    throw new ArgumentOutOfRangeException();

                case "PageAction":
                    return asJObject.ToObject<PageActionButton>();

                case "ModalAction":
                    var children = ComponentParser.ParseComponents(asJObject["components"].ToString());
                    var modalActionButton = asJObject.ToObject<ModalActionButton>();
                    modalActionButton.Components = children;
                    return modalActionButton;

                case "Navigation":
                    return asJObject.ToObject<NavigationButton>();
            }
        }
    }

    public class NavigationButton : Button
    {
        public NavigationButton(string navigateTo, string text, string style) 
            : base(ButtonType.Navigation, text, style, navigateTo: navigateTo)
        {
        }
    }

    public class PageActionButton : Button
    {
        public string Action { get; }

        /// <summary>
        /// Keys for data to be sent in action request.
        /// </summary>
        public List<string> ActionDataKeys { get; }

        /// <summary>
        /// List of id's for components to include in request.
        /// </summary>
        public List<string> ComponentsToInclude { get; }

        public PageActionButton(
            string action, 
            List<string> actionDataKeys, 
            string text, 
            string style = "outline-primary",
            bool requireConfirm = false,
            string navigateTo = null,
            List<string> componentsToInclude = null)
            : base(ButtonType.PageAction, text, style, requireConfirm, navigateTo)
        {
            Action = action;
            ActionDataKeys = actionDataKeys;
            ComponentsToInclude = componentsToInclude;
        }
    }

    /// <summary>
    /// Button which opens a modal for performing action
    /// </summary>
    public class ModalActionButton : Button
    {
        public string Action { get; set; }

        /// <summary>
        /// Modal fields.
        /// </summary>
        public List<Component> Components { get; set; }

        public ModalActionButton(
            string action, 
            List<Component> components,
            string text,
            string style = "primary",
            bool requireConfirm = false,
            string navigateTo = null)
            : base(ButtonType.ModalAction, text, style, requireConfirm, navigateTo)
        {
            Action = action;
            Components = components;
        }
    }
}
