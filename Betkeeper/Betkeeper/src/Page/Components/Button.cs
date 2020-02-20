﻿using System.Collections.Generic;
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

    public enum FieldType
    {
        DateTime,
        Integer,
        Double,
        TextBox,
        TextArea
    }

    public class Button : Component
    {
        public string Text { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ButtonType ButtonType { get; }

        public string Style { get; }

        public Button (ButtonType buttonType, string text, string style = "primary")
            : base(ComponentType.Button)
        {
            Text = text;
            ButtonType = ButtonType;
            Style = style;
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
        public string ActionUrl { get; }

        /// <summary>
        /// Keys for data to be sent in action request.
        /// </summary>
        public List<string> ActionDataKeys { get; }

        public PageActionButton(string actionUrl, List<string> actionDataKeys, string text, string style)
            : base(ButtonType.PageAction, text, style)
        {
            ActionUrl = actionUrl;
            ActionDataKeys = actionDataKeys;
        }
    }


    /// <summary>
    /// Button which opens a modal for performing action
    /// </summary>
    public class ModalActionButton : Button
    {
        public string ActionUrl { get; }

        /// <summary>
        /// Modal fields. Key is the key to be sent in data.
        /// </summary>
        public Dictionary<string, FieldType> ModalFields { get; }

        public ModalActionButton(
            string actionUrl, 
            Dictionary<string, FieldType> modalFields,
            string text)
            : base(ButtonType.ModalAction, text)
        {
            ActionUrl = actionUrl;
            ModalFields = modalFields;
        }
    }
}