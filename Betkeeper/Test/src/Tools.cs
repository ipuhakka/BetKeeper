using Betkeeper.Actions;
using Betkeeper.Data;
using Betkeeper.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace TestTools
{
    public static class Tools
    {
        /// <summary>
        /// Mocks http controller context
        /// </summary>
        /// <param name="dataContent">Mock request content</param>
        /// <param name="headers">Mock request headers</param>
        /// <returns></returns>
        public static HttpControllerContext MockHttpControllerContext(
            object dataContent = null,
            Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(dataContent))
            };

            if (headers != null)
            {
                foreach (var headerDictRow in headers)
                {
                    request.Headers.Add(headerDictRow.Key, headerDictRow.Value);
                }
            }

            return new HttpControllerContext
            {
                Request = request
            };
        }

        /// <summary>
        /// Creates test data for given context.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="participators"></param>
        /// <param name="competitions"></param>
        /// <param name="users"></param>
        /// <param name="targets"></param>
        public static void CreateTestData(
           BetkeeperDataContext context,
           List<Participator> participators = null,
           List<Competition> competitions = null,
           List<User> users = null,
           List<Target> targets = null)
        {
            if (participators != null)
            {
                participators.ForEach(participator =>
                    context.Participator.Add(participator));
            }

            if (competitions != null)
            {
                competitions.ForEach(competition =>
                    context.Competition.Add(competition));
            }

            if (users != null)
            {
                users.ForEach(user =>
                    context.User.Add(user));
            }

            if (targets != null)
            {
                targets.ForEach(target =>
                    context.Target.Add(target));
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Creates test context using in memory database.
        /// </summary>
        /// <returns></returns>
        public static BetkeeperDataContext GetTestContext()
        {
            return new BetkeeperDataContext(GetTestOptionsBuilder());
        }

        private static DbContextOptionsBuilder GetTestOptionsBuilder()
        {
            return new DbContextOptionsBuilder()
                .UseInMemoryDatabase("TestDatabase");
        }
    }
}
