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
        /// Creates test data.
        /// </summary>
        /// <param name="participators"></param>
        /// <param name="competitions"></param>
        /// <param name="users"></param>
        /// <param name="targets"></param>
        /// <param name="targetBets"></param>
        public static void CreateTestData(
           List<Participator> participators = null,
           List<Competition> competitions = null,
           List<User> users = null,
           List<Target> targets = null,
           List<TargetBet> targetBets = null)
        {
            var context = GetTestContext();
            if (participators != null)
            {
                context.Participator.AddRange(participators);
            }

            if (competitions != null)
            {
                context.Competition.AddRange(competitions);
            }

            if (users != null)
            {
                context.User.AddRange(users);
            }

            if (targets != null)
            {
                context.Target.AddRange(targets);
            }

            if (targetBets != null)
            {
                context.TargetBet.AddRange(targetBets);
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
