using Betkeeper.Page.Components;
using System.Collections.Generic;

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
            :this(userId, page, action, parameters)
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
        /// Constructor for page action response to return components.
        /// </summary>
        /// <param name="components"></param>
        public PageActionResponse(List<Component> components)
        {
            Components = components;
        }

        /// <summary>
        /// Constructor for page action response with a single component.
        /// </summary>
        /// <param name="components"></param>
        public PageActionResponse(Component component)
        {
            Components = new List<Component>();
            Components.Add(component);
        }
    }
}
