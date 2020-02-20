using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Betkeeper.Classes;
using Betkeeper.Page.Components;

namespace Betkeeper.Page
{
    public class PageResponse
    {
        public string Key { get; set; }

        public List<Component> Components { get; set; }

        public Dictionary<string, object> Data { get; set; }

        public PageResponse(string pageKey, List<Component> components)
        {
            Key = pageKey;
            Components = components;
            // TODO: Data voisi olla aina oma luokkarakenteensa joka vastaukselle.
            Data = new Dictionary<string, object>();
        }

        public static HttpResponseMessage GetResponseMessage(string pageKey, int userId)
        {
            switch (pageKey)
            {
                default:
                    return Http.CreateResponse(HttpStatusCode.NotFound);

                case "competitions":
                    return CompetitionPage.GetCompetitionResponse(pageKey);
            }
        }
    }
}
