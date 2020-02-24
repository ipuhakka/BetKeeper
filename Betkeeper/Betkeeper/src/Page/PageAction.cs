using System.Collections.Generic;

namespace Betkeeper.Page
{
    public class PageAction
    {
        public int UserId { get; }

        public Dictionary<string, object> Parameters { get; }

        public string Page { get; }

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
    }
}
