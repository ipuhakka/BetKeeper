using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Betkeeper.Classes;
using Betkeeper.Page.Components;
using Betkeeper.Pages;

namespace Betkeeper.Page
{
    public interface IPage
    {

        /// <summary>
        /// Get page structure and data.
        /// </summary>
        /// <param name="pageKey"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        HttpResponseMessage GetResponse(string pageKey, int userId);

        /// <summary>
        /// Handles a page action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        HttpResponseMessage HandleAction(PageAction action);

        /// <summary>
        /// Handle a dropdown value update. 
        /// Allows to change UI with dropdown value change.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        HttpResponseMessage HandleDropdownUpdate(
            Dictionary<string, object> data, 
            int? pageId = null);
    }

    public class PageResponse
    {
        public string Key { get; set; }

        public List<Component> Components { get; set; }

        public Dictionary<string, object> Data { get; set; }

        public PageResponse(
            string pageKey, 
            List<Component> components,
            Dictionary<string, object> data)
        {
            Key = pageKey;
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

        public static HttpResponseMessage GetResponseMessage(
            string pageKey, 
            int userId, 
            int? pageId = null)
        {
            switch (pageKey)
            {
                default:
                    return Http.CreateResponse(HttpStatusCode.NotFound);

                case "competitions":
                    return pageId != null
                        ? new CompetitionPage().GetResponse(pageId.ToString(), userId)
                        : new CompetitionsPage().GetResponse(pageKey, userId);
            }
        }

        /// <summary>
        /// Executes a page action.
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="action">Action name.</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static HttpResponseMessage HandlePageAction(PageAction action)
        {
            switch (action.Page)
            {
                default:
                    return Http.CreateResponse(HttpStatusCode.NotFound);

                case "competitions":
                    if (action.PageId != null)
                    {
                        return new CompetitionPage().HandleAction(action);
                    }

                    return new CompetitionsPage().HandleAction(action);
            }
        }

        public static IPage GetPageInstance(string page, int? id = null)
        {
            switch (page)
            {
                default:
                    throw new ArgumentException($"{page} not found");

                case "competitions":
                    if (id != null)
                    {
                        return new CompetitionPage();
                    }

                    return new CompetitionsPage();
            }
        }
    }
}
