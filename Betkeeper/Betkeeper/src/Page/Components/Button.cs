using Betkeeper.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Betkeeper.Page.Components
{
    public enum ButtonType
    {
        PageAction,
        ModalAction,
        Navigation
    }

    public enum DisplayType
    {
        Text,
        Icon
    }

    [Serializable]
    public abstract class Button : Component
    {
        public string Text { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ButtonType ButtonType { get; }

        public string Style { get; }

        public bool RequireConfirm { get; }

        public string NavigateTo { get; }

        public string IconName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DisplayType DisplayType { get; set; }

        public Button(
            ButtonType buttonType,
            string text,
            string style = "primary",
            bool requireConfirm = false,
            string navigateTo = null,
            DisplayType displayType = DisplayType.Text)
            : base(ComponentType.Button)
        {
            Text = text;
            ButtonType = buttonType;
            Style = style;
            RequireConfirm = requireConfirm;
            NavigateTo = navigateTo;
            DisplayType = displayType;
        }

        /// <summary>
        /// Parses a button from jObject
        /// </summary>
        /// <param name="asJObject"></param>
        /// <returns></returns>
        public static Button Parse(JObject asJObject)
        {
            var buttonType = EnumHelper.FromString<ButtonType>(asJObject["buttonType"].ToString());

            switch (buttonType)
            {
                default:
                    throw new ArgumentOutOfRangeException();

                case ButtonType.PageAction:
                    return asJObject.ToObject<PageActionButton>();

                case ButtonType.ModalAction:
                    var children = Component.ParseComponents(asJObject["components"].ToString());
                    var modalActionButton = asJObject.ToObject<ModalActionButton>();
                    modalActionButton.Components = children;
                    return modalActionButton;

                case ButtonType.Navigation:
                    return asJObject.ToObject<NavigationButton>();
            }
        }
    }

    [Serializable]
    public class NavigationButton : Button
    {
        public NavigationButton(string navigateTo, string text, string style)
            : base(ButtonType.Navigation, text, style, navigateTo: navigateTo)
        {
        }
    }

    [Serializable]
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
            List<string> componentsToInclude = null,
            DisplayType displayType = DisplayType.Text)
            : base(ButtonType.PageAction, text, style, requireConfirm, navigateTo, displayType)
        {
            Action = action;
            ActionDataKeys = actionDataKeys;
            ComponentsToInclude = componentsToInclude;
        }
    }

    [Serializable]
    /// <summary>
    /// Button which opens a modal for performing action
    /// </summary>
    public class ModalActionButton : Button
    {
        public string Action { get; set; }

        /// <summary>
        /// Root for data path. Used to set page content to look data from correct data path
        /// </summary>
        public string AbsoluteDataPath { get; set; }

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
            string navigateTo = null,
            string absoluteDataPath = null)
            : base(ButtonType.ModalAction, text, style, requireConfirm, navigateTo)
        {
            Action = action;
            Components = components;
            AbsoluteDataPath = absoluteDataPath;
        }
    }
}
