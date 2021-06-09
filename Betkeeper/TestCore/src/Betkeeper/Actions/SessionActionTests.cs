using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Betkeeper.Classes;
using Betkeeper.Actions;
using Betkeeper.Models;
using TestTools;
using Betkeeper.Data;
using Microsoft.AspNetCore.Mvc;

namespace Betkeeper.Test.Betkeeper.Actions
{
    [TestFixture]
    public class SessionActionTests
    {
        private BetkeeperDataContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = Tools.GetTestContext();
            Settings.InitializeOptionsBuilderService(Tools.GetTestOptionsBuilder());
        }

        [TearDown]
        public void TearDown()
        {
            _context.Sessions.RemoveRange(_context.Sessions);
            _context.SaveChanges();
        }

        [Test]
        public void InstantiateSession_NoExistingSession_NewOneCreated()
        {
            var token = SessionAction.InstantiateSession(1);

            Assert.AreEqual(24, token.TokenString.Length, "Token length");
            Assert.AreEqual(1, token.Owner, "Token owner");

            var sessions = _context.Sessions.ToList();
            Assert.AreEqual(1, sessions.Count, "Session count");

            Assert.AreEqual(1, sessions[0].UserId, "Session user");
            Assert.AreEqual(token.TokenString, sessions[0].Token, "Session token");
        }

        [Test]
        public void InstantiateSession_SessionNotExpired_SessionNotUpdated()
        {
            var session = new Session
            {
                UserId = 1,
                Token = "asdasdasdasdasdasdasdasd",
                ExpirationTime = DateTime.UtcNow.AddHours(2)
            };

            var testSessions = new List<Session>
            {
                session
            };

            Tools.CreateTestData(sessions: testSessions);

            SessionAction.InstantiateSession(1);

            var sessions = _context.Sessions.ToList();
            Assert.AreEqual(1, sessions.Count, "Session count");

            Assert.AreEqual(session.Token, sessions[0].Token, "Session token");
            Assert.AreEqual(session.ExpirationTime, sessions[0].ExpirationTime, "Expiration time");
        }

        [Test]
        public void InstantiateSession_SessionExpired_SessionUpdated()
        {
            var session = new Session
            {
                UserId = 1,
                Token = "asdasdasdasdasdasdasdasd",
                ExpirationTime = DateTime.UtcNow.AddHours(-2)
            };

            var testSessions = new List<Session>
            {
                session
            };

            Tools.CreateTestData(sessions: testSessions);

            SessionAction.InstantiateSession(1);

            var sessions = _context.Sessions.ToList();
            Assert.AreEqual(1, sessions.Count, "Session count");

            Assert.AreEqual(session.UserId, sessions[0].UserId, "User id");
            Assert.AreNotEqual(session.Token, sessions[0].Token, "Session token");
            Assert.AreNotEqual(session.ExpirationTime, sessions[0].ExpirationTime, "Expiration time");
        }

        [Test]
        public void SessionActive_SessionDoesNotExist_ReturnsFalse()
        {
            Assert.IsFalse(SessionAction.SessionActive(1, "asdasdasdasdasdasdasd"));
        }

        [Test]
        public void SessionActive_SessionExpired_ReturnsFalse()
        {
            var testSessions = new List<Session>
            {
                new Session
                {
                    UserId = 1,
                    Token = "asdasdasdasdasdasdasdasd",
                    ExpirationTime = DateTime.UtcNow.AddHours(-2)
                }
            };

            Tools.CreateTestData(sessions: testSessions);

            Assert.IsFalse(SessionAction.SessionActive(1, "asdasdasdasdasdasdasdasd"));
        }

        [Test]
        public void SessionActive_SessionNotExpired_ReturnsTrue()
        {
            var testSessions = new List<Session>
            {
                new Session
                {
                    UserId = 1,
                    Token = "asdasdasdasdasdasdasdasd",
                    ExpirationTime = DateTime.UtcNow.AddHours(2)
                }
            };

            Tools.CreateTestData(sessions: testSessions);

            Assert.IsTrue(SessionAction.SessionActive(1, "asdasdasdasdasdasdasdasd"));
        }

        [Test]
        public void GetUserIdFromRequest_UserDoesNotHaveToken_ReturnsNull()
        {
            var controller = new TestController
            {
                ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "tokenDoesNotExist" }
                    })
            };

            Assert.IsNull(SessionAction.GetUserIdFromRequest(controller.Request));
        }


        [Test]
        public void GetUserIdFromRequest_UserHasToken_ReturnsUserId()
        {
            var testSession = Session.GenerateSession(new Token(2));

            var testSessions = new List<Session>
            {
                new Session
                {
                    UserId = 1,
                    Token = "asdasdasdasdasdasdasdasd",
                    ExpirationTime = DateTime.UtcNow.AddHours(2)
                },
                testSession
            };

            Tools.CreateTestData(sessions: testSessions);

            var controller = new TestController
            {
                ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", testSession.Token}
                    })
            };

            Assert.AreEqual(2, SessionAction.GetUserIdFromRequest(controller.Request));
        }

        [Test]
        public void DeleteSession_SessionMatches_SessionDeleted()
        {
            var session = Session.GenerateSession(new Token(1));

            var sessions = new List<Session>
            {
                Session.GenerateSession(new Token(2)),
                session
            };

            Tools.CreateTestData(sessions: sessions);

            SessionAction.DeleteSession(1, session.Token);

            var remainingSessions = _context.Sessions.ToList();

            Assert.AreEqual(1, remainingSessions.Count, "Session count");

            Assert.AreEqual(2, remainingSessions[0].UserId, "Session user");
        }

        [Test]
        public void DeleteSession_SessionDoesNotMatch_SessionNotDeleted()
        {
            var session = Session.GenerateSession(new Token(1));

            var sessions = new List<Session>
            {
                Session.GenerateSession(new Token(2)),
                session
            };

            Tools.CreateTestData(sessions: sessions);

            SessionAction.DeleteSession(1, "notexistingtoken");

            var remainingSessions = _context.Sessions.ToList();

            Assert.AreEqual(2, remainingSessions.Count, "Session count");
        }

        private class TestController : ControllerBase
        {

        }
    }
}
