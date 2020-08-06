using System;
using System.Collections.Generic;
using System.Linq;
using Betkeeper.Data;
using Betkeeper.Models;
using NUnit.Framework;
using TestTools;

namespace Betkeeper.Test.Models
{
    [TestFixture]
    public class TargetBetTests
    {
        private TargetBetRepository _targetBetRepository;
        private BetkeeperDataContext _context;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Set Connectionstring so base constructor runs
            Settings.ConnectionString = "TestDatabase";
            _context = Tools.GetTestContext();
            _targetBetRepository = new TargetBetRepository(_context);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _targetBetRepository.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            _context.TargetBet.RemoveRange(_context.TargetBet);
        }

        [Test]
        public void AddTargets_AddsTargets()
        {
            var targetBets = new List<TargetBet>
            {
                new TargetBet
                {
                    TargetBetId = 1,
                    Participator = 1,
                    Target = 1,
                    Bet = "Bet"
                },
                new TargetBet
                {
                    TargetBetId = 2,
                    Participator = 1,
                    Target = 1,
                    Bet = "Bet"
                },
            };

            _targetBetRepository.AddTargetBets(targetBets);

            Assert.AreEqual(2, _context.TargetBet.Count());
        }

        [Test]
        public void AddTargets_SameKeyThrowsInvalidOperationException()
        {
            var targetBets = new List<TargetBet>
            {
                new TargetBet
                {
                    TargetBetId = 1,
                    Participator = 1,
                    Target = 1,
                    Bet = "Bet"
                },
                new TargetBet
                {
                    TargetBetId = 1,
                    Participator = 1,
                    Target = 1,
                    Bet = "Bet"
                },
            };

            Assert.Throws<InvalidOperationException>(() =>
            _targetBetRepository.AddTargetBets(targetBets));
        }
    }
}
