using Betkeeper.Page.Components;
using System;

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
        public virtual PageActionResponse HandleAction(PageAction action)
        {
            throw new NotImplementedException("Page does not implement actions");
        }

        /// <summary>
        /// Handle a dropdown value update. 
        /// Allows to change UI with dropdown value change.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual PageResponse HandleDropdownUpdate(DropdownUpdateParameters parameters)
        {
            throw new NotImplementedException("Page does not implement handling dropdown updates");
        }

        /// <summary>
        /// Method called on expanding an expandable list group item
        /// </summary>
        /// <param name="componentKey"></param>
        /// <param name="itemKey"></param>
        public virtual ItemContent ExpandListGroupItem(ListGroupItemExpandParameters expandParameters)
        {
            throw new NotImplementedException("Page does not implement handling dropdown updates");
        }
    }
}
