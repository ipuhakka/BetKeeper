using Betkeeper.Classes;
using Betkeeper.Page.Components;
using Betkeeper.Pages;
using Betkeeper.Pages.CompetitionPage;
using Betkeeper.Pages.FoldersPage;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Betkeeper.Page
{
    public interface IPage
    {
        /// <summary>
        /// Gets page structure 
        /// </summary>
        /// <param name="pageKey"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        PageResponse GetPage(string pageKey, int userId);

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
            PageResponse pageResponse;
            switch (pageKey)
            {
                default:
                    return Http.CreateResponse(HttpStatusCode.NotFound);

                case "competitions":
                    pageResponse = pageId != null
                        ? new CompetitionPage().GetPage(pageId.ToString(), userId)
                        : new CompetitionsPage().GetPage(pageKey, userId);
                    break;

                case "usersettings":
                    pageResponse = new UserSettingsPage().GetPage(pageKey, userId);
                    break;

                case "folders":
                    pageResponse = new FoldersPage().GetPage(pageKey, userId);
                    break;
            }

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

                case "usersettings":
                    return new UserSettingsPage().HandleAction(action);
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

                case "usersettings":
                    return new UserSettingsPage();
            }
        }
    }
}
