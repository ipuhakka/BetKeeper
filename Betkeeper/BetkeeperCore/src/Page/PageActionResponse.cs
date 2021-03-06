﻿using Betkeeper.Classes;
using Betkeeper.Enums;
using Betkeeper.Page.Components;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Betkeeper.Page
{
    /// <summary>
    /// Page action response model.
    /// </summary>
    public class PageActionResponse
    {
        /// <summary>
        /// List of components to update. 
        /// This does not allow for deleting or creating completely new components.
        /// Modification is restricted to inside the component.
        /// </summary>
        public List<Component> Components { get; set; }

        /// <summary>
        /// Data object
        /// </summary>
        public Dictionary<string, object> Data { get; set; }

        /// <summary>
        /// Action message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Show alert after completing the action.
        /// </summary>
        public bool ShowAlert { get; }

        /// <summary>
        /// Refresh page after completing the action.
        /// </summary>
        public bool Refresh { get; set; }

        [JsonIgnore]
        public ActionResultType ActionResultType { get; }

        /// <summary>
        /// Constructor with only a result type
        /// </summary>
        /// <param name="actionResultType"></param>
        public PageActionResponse(ActionResultType actionResultType)
        {
            ActionResultType = actionResultType;
        }

        /// <summary>
        /// Creates a pageaction response with message to be displayed
        /// </summary>
        /// param name="actionResult"></param>
        /// <param name="message"></param>
        /// <param name="refresh"></param>
        public PageActionResponse(
            ActionResultType actionResult,
            string message,
            bool refresh = false)
        {
            ActionResultType = actionResult;
            Message = message;
            ShowAlert = true;
            Refresh = refresh;
        }

        /// <summary>
        /// Constructor for page action response with a single component.
        /// </summary>
        /// <param name="components"></param>
        public PageActionResponse(Component component)
        {
            ActionResultType = ActionResultType.OK;
            Components = new List<Component>
            {
                component
            };
        }
    }
}
