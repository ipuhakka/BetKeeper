using Betkeeper.Classes;
using Betkeeper.Enums;
using Betkeeper.Page.Components;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Betkeeper.Page
{
    public class PageAction
    {
        public int UserId { get; }

        public Dictionary<string, object> Parameters { get; }

        public string Page { get; }

        public int? PageId { get; }

        public string ActionName { get; }

        public PageAction(
            int userId,
            string page,
            string action,
            Dictionary<string, object> parameters)
        {
            UserId = userId;
            Page = page;
            ActionName = action;
            Parameters = parameters;
        }

        /// <summary>
        /// Constructor for id based action.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="action"></param>
        /// <param name="parameters"></param>
        /// <param name="pageId"></param>
        public PageAction(
            int userId,
            string page,
            string action,
            Dictionary<string, object> parameters,
            int pageId)
            : this(userId, page, action, parameters)
        {
            PageId = pageId;
        }
    }

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

        private ActionResultType ActionResultType { get; }

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
        /// Constructor for page action response to return components.
        /// </summary>
        /// <param name="components"></param>
        public PageActionResponse(List<Component> components)
        {
            ActionResultType = ActionResultType.OK;
            Components = components;
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

        /// <summary>
        /// Creates a Http response from page action response
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage ToHttpResponseMessage()
        {
            return Http.CreateResponse(
                (HttpStatusCode)ActionResultType, 
                this);
        }
    }
}
