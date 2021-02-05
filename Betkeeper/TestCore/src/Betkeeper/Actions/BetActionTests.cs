using System;
using System.Collections.Generic;
using System.Linq;
using Betkeeper.Actions;
using Betkeeper.Data;
using Betkeeper.Enums;
using Betkeeper.Models;
using NUnit.Framework;
using TestTools;

namespace Betkeeper.Test.Actions
{
    [TestFixture]
    public class BetActionTests
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
            _context.Bet.RemoveRange(_context.Bet);
            _context.User.RemoveRange(_context.User);
            _context.SaveChanges();
        }

        [Test]
        public void CreateBetUserDoesNotExist_ThrowsActionException()
        {
            try
            {
                new BetAction().CreateBet(
                    BetResult.Lost,
                    "",
                    2,
                    2,
                    DateTime.Now,
                    1);

                Assert.Fail();
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionResultType.Conflict, e.ActionExceptionType);
                Assert.AreEqual("User not found", e.ErrorMessage);
            }
        }

        [Test]
        public void CreateBet_UserExists_BetCreated()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1
                }
            };

            Tools.CreateTestData(users: users);

            new BetAction().CreateBet(
                BetResult.Lost,
                "name",
                2,
                1,
                DateTime.Today,
                1);

            var bets = _context.Bet.ToList();

            Assert.AreEqual(1, bets.Count);
            Assert.AreEqual(DateTime.Today, bets[0].PlayedDate);
            Assert.AreEqual("name", bets[0].Name);
        }

        [Test]
        public void DeleteBet_UserDoesNotHaveBet_ThrowsActionException()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1
                },
                new User
                {
                    UserId = 2
                }
            };

            var bets = new List<Bet>
            {
                new Bet
                {
                    BetId = 1,
                    Owner = 2
                }
            };

            Tools.CreateTestData(users: users, bets: bets);

            try
            {
                new BetAction().DeleteBet(1, 1);
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionResultType.NotFound, e.ActionExceptionType);
                Assert.AreEqual("Bet trying to be deleted was not found", e.ErrorMessage);
            }
        }

        [Test]
        public void DeleteBet_UserHasBet_BetDeleted()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1
                }
            };

            var bets = new List<Bet>
            {
                new Bet
                {
                    BetId = 1,
                    Owner = 1
                }
            };

            Tools.CreateTestData(users: users, bets: bets);

            new BetAction().DeleteBet(1, 1);

            Assert.AreEqual(0, _context.Bet.Count());
        }

        [Test]
        public void ModifyBet_BetNotFound_ThrowsActionException()
        {
            try
            {
                new BetAction().ModifyBet(
                    1,
                    1);

                Assert.Fail();
            }
            catch(ActionException e)
            {
                Assert.AreEqual(ActionResultType.NotFound, e.ActionExceptionType);
            }
        }

        [Test]
        public void ModifyBet_AllParametersNull_NothingModified()
        {
            var bets = new List<Bet>
            {
                new Bet
                {
                    BetId = 1,
                    BetResult = BetResult.Won,
                    Owner = 1,
                    Odd = 1,
                    Stake = 1,
                    Name = "test"
                }
            };

            Tools.CreateTestData(bets: bets);

            new BetAction().ModifyBet(1, 1);

            var bet = _context.Bet.Single(betEntity => betEntity.BetId == 1);

            Assert.AreEqual("test", bet.Name);
            Assert.AreEqual(BetResult.Won, bet.BetResult);
            Assert.AreEqual(1, bet.Odd);
            Assert.AreEqual(1, bet.Stake);
        }

        [Test]
        public void ModifyBet_ModifiesValues()
        {
            var bets = new List<Bet>
            {
                new Bet
                {
                    BetId = 1,
                    BetResult = BetResult.Won,
                    Owner = 1,
                    Odd = 1,
                    Stake = 1,
                    Name = "test"
                }
            };

            Tools.CreateTestData(bets: bets);

            new BetAction().ModifyBet(
                1, 
                1,
                BetResult.Lost,
                2,
                3,
                "test2");

            var bet = _context.Bet.Single(betEntity => betEntity.BetId == 1);

            Assert.AreEqual("test2", bet.Name);
            Assert.AreEqual(BetResult.Lost, bet.BetResult);
            Assert.AreEqual(3, bet.Odd);
            Assert.AreEqual(2, bet.Stake);
        }
    }
}
