using Betkeeper;
using Betkeeper.Data;
using Betkeeper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace TestTools
{
    public static class Tools
    {
        /// <summary>
        /// Mock controller context
        /// </summary>
        /// <param name="dataContent"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static ControllerContext MockControllerContext(
            object dataContent = null,
            Dictionary<string, string> headers = null)
        {
            var httpContext = new DefaultHttpContext();
            
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpContext.Request.Headers[header.Key] = header.Value;
                }
            }

            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dataContent)));

            return new ControllerContext
            {
                HttpContext = httpContext
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
        /// <param name="folders"></param>
        /// <param name="betInBetFolders"></param>
        /// <param name="bets"></param>
        /// <param name="invitations"></param>
        public static void CreateTestData(
           List<Participator> participators = null,
           List<Competition> competitions = null,
           List<User> users = null,
           List<Target> targets = null,
           List<TargetBet> targetBets = null,
           List<Folder> folders = null,
           List<BetInBetFolder> betInBetFolders = null,
           List<Bet> bets = null,
           List<CompetitionInvitation> invitations = null)
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

            if (folders != null)
            {
                context.Folder.AddRange(folders);
            }

            if (betInBetFolders != null)
            {
                context.BetInBetFolder.AddRange(betInBetFolders);
            }

            if (bets != null)
            {
                context.Bet.AddRange(bets);
            }

            if (invitations != null)
            {
                context.CompetitionInvitation.AddRange(invitations);
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

        public static DbContextOptionsBuilder GetTestOptionsBuilder()
        {
            return new DbContextOptionsBuilder()
                .UseInMemoryDatabase("TestDatabase");
        }

        public static void InitTestSecretKey()
        {
            Settings.SecretKey = "iwqqernnjnvhlyquwqulwueuyvxvycuz";
        }
    }
}
