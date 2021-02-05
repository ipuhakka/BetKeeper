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
            : this(userId, page, action, parameters)
        {
            PageId = pageId;
        }
    }
}
