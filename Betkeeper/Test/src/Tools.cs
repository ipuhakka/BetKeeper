using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Controllers;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Betkeeper.Data;
using Betkeeper.Models;

namespace TestTools
{
    public static class Tools
    {
        public static HttpControllerContext MockHttpControllerContext(
            object dataContent = null, 
            Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage();

            request.Content = new StringContent(JsonConvert.SerializeObject(dataContent));

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

        public static void CreateTestData(
            List<Participator> participators = null,
            List<Competition> competitions = null)
        {
            using (var context = new BetkeeperDataContext(
               GetTestOptionsBuilder()))
            {
                context.Database.EnsureCreated();

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

                context.SaveChanges();
            }
        }

        public static DbContextOptionsBuilder GetTestOptionsBuilder()
        {
            return new DbContextOptionsBuilder()
                .UseInMemoryDatabase("TestDatabase");
        }
    }

    /// <summary>
    /// Testable implementation of competition repository.
    /// </summary>
    internal class TestCompetitionRepository : CompetitionRepository
    {
        internal TestCompetitionRepository()
        {
            OptionsBuilder = Tools.GetTestOptionsBuilder();
        }
    }

    /// <summary>
    /// Testable implementation of participator repository.
    /// </summary>
    internal class TestParticipatorRepository : ParticipatorRepository
    {
        internal TestParticipatorRepository()
        {
            OptionsBuilder = Tools.GetTestOptionsBuilder();
            CompetitionHandler = new TestCompetitionRepository();
        }
    }
}
