using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Betkeeper.Classes;
using Betkeeper.Page.Components;

namespace Betkeeper.Page
{
    public abstract class Page
    {

        /// <summary>
        /// Get page structure and data.
        /// </summary>
        /// <param name="pageKey"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public abstract PageResponse GetResponse(string pageKey, int userId);

        /// <summary>
        /// Handles a page action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public abstract HttpResponseMessage HandleAction(PageAction action);
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

        public static HttpResponseMessage GetResponseMessage(string pageKey, int userId)
        {
            switch (pageKey)
            {
                default:
                    return Http.CreateResponse(HttpStatusCode.NotFound);

                case "competitions":
                    return Http.CreateResponse(
                        HttpStatusCode.OK,
                        new CompetitionsPage().GetResponse(pageKey, userId));
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
                    return new CompetitionsPage().HandleAction(action);
            }
        }
    }
}
