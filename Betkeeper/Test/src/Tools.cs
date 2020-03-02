using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Controllers;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Betkeeper.Data;
using Betkeeper.Models;
using Betkeeper.Actions;

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
            List<Competition> competitions = null,
            List<User> users = null,
            List<Target> targets = null)
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
        }

        public static DbContextOptionsBuilder GetTestOptionsBuilder()
        {
            return new DbContextOptionsBuilder()
                .UseInMemoryDatabase("TestDatabase");
        }
    }

    internal class TestTargetRepository : TargetRepository
    {
        internal TestTargetRepository()
        {
            OptionsBuilder = Tools.GetTestOptionsBuilder();
        }
    }


    internal class TestUserRepository : UserRepository
    {
        internal TestUserRepository()
        {
            OptionsBuilder = Tools.GetTestOptionsBuilder();
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

    /// <summary>
    /// Testable implementation of competition action.
    /// </summary>
    internal class TestCompetitionAction : CompetitionAction
    {
        public TestCompetitionAction()
        {
            CompetitionRepository = new TestCompetitionRepository();
            ParticipatorRepository = new TestParticipatorRepository();
        }
    }

    /// <summary>
    /// Testable implementation of target action.
    /// </summary>
    internal class TestTargetAction : TargetAction
    {
        public TestTargetAction()
        {
            CompetitionRepository = new TestCompetitionRepository();
            ParticipatorRepository = new TestParticipatorRepository();
            TargetRepository = new TestTargetRepository();
        }
    }
}
