using Betkeeper.Classes;
using Betkeeper.Services;
using Betkeeper.Page.Components;
using Betkeeper.Pages;
using Betkeeper.Pages.BetsPage;
using Betkeeper.Pages.CompetitionPage;
using Betkeeper.Pages.FoldersPage;
using Betkeeper.Pages.HomePage;
using Betkeeper.Pages.StatisticsPage;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

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

        public static HttpResponseMessage GetResponseMessage(
            string pageKey,
            int userId,
            int? pageId = null)
        {
            var pageInstance = PageService.GetPageInstance(pageKey);

            if (pageInstance == null)
            {
                return Http.CreateResponse(HttpStatusCode.NotFound);
            }

            // Use page id by default, if it is null use pageKey
            var pageResponse = pageInstance.GetPage(pageId?.ToString() ?? pageKey, userId);

            if (pageResponse.Redirect)
            {
                var response = Http.CreateResponse(HttpStatusCode.Redirect);
                response.Headers.Add("Location", pageResponse.RedirectTo);

                return response;
            }

            return Http.CreateResponse(HttpStatusCode.OK, pageResponse);
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
            var pageInstance = PageService.GetPageInstance(action.Page);

            if (pageInstance == null)
            {
                return Http.CreateResponse(HttpStatusCode.NotFound);
            }

            return pageInstance.HandleAction(action).ToHttpResponseMessage();
        }
    }
}
