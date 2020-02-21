using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Betkeeper.Classes;
using Betkeeper.Page.Components;

namespace Betkeeper.Page
{
    public class CompetitionPage
    {

        public static HttpResponseMessage GetCompetitionResponse(string pageKey)
        {
            var components = new List<Component>
            {
                new Input("TestInput", InputType.TextBox),
                new ModalActionButton(
                    "competitions/post", 
                    new List<Field>
                    {
                        new Field("Name", "Name", FieldType.TextBox),
                        new Field("StartTime", "Start time", FieldType.DateTime),
                        new Field("Description", "Description", FieldType.TextArea)
                    },
                    "Create a competition")
            };

            return Http.CreateResponse(HttpStatusCode.OK, new PageResponse(pageKey, components));
        }
    }
}
