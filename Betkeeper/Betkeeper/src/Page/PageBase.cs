using System;
using System.Collections.Generic;

namespace Betkeeper.Page
{
    /// <summary>
    /// Page base class
    /// </summary>
    public abstract class PageBase
    {
        /// <summary>
        /// Gets page structure 
        /// </summary>
        /// <param name="pageKey"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public abstract PageResponse GetPage(string pageKey, int userId);

        /// <summary>
        /// Handles a page action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public abstract PageActionResponse HandleAction(PageAction action);

        /// <summary>
        /// Handle a dropdown value update. 
        /// Allows to change UI with dropdown value change.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public virtual PageResponse HandleDropdownUpdate(
            Dictionary<string, object> data,
            int? pageId = null)
        {
            throw new NotImplementedException("Page does not implement handling dropdown updates");
        }
    }
}
