using Betkeeper.Page.Components;
using System.Collections.Generic;

namespace Betkeeper.Page
{
    public class PageResponse
    {
        public string PageKey { get; set; }

        public List<Component> Components { get; set; }

        public Dictionary<string, object> Data { get; set; }

        public bool Redirect { get; set; }

        public string RedirectTo { get; set; }

        public PageResponse(
            string pageKey,
            List<Component> components,
            Dictionary<string, object> data)
        {
            PageKey = pageKey;
            Components = components;
            Data = data;
        }

        /// <summary>
        /// Constructor for page response to update components.
        /// </summary>
        /// <param name="components"></param>
        public PageResponse(List<Component> components)
        {
            Components = components;
        }

        /// <summary>
        /// Constructor for creating a page response which should redirect user to another url.
        /// </summary>
        /// <param name="redirectTo"></param>
        public PageResponse(string redirectTo)
        {
            Redirect = true;
            RedirectTo = redirectTo;
        }
    }
}
