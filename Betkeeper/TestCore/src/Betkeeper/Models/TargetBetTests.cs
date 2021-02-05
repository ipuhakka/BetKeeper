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

        [SetUp]
        public void SetUp()
        {
            _context = Tools.GetTestContext();
            _targetBetRepository = new TargetBetRepository();
        }

        [TearDown]
        public void TearDown()
        {
            _context.TargetBet.RemoveRange(_context.TargetBet);
            _context.SaveChanges();
        }

        [Test]
        public void AddTargetBets_AddsTargetBets()
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
        public void AddTargetBets_NoParticipator_ThrowsInvalidOperationException()
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
                    Target = 1,
                    Bet = "Bet"
                }
            };

            Assert.Throws<InvalidOperationException>(() =>
                _targetBetRepository.AddTargetBets(targetBets));
        }

        [Test]
        public void UpdateTargetBets_NoParticipator_ThrowsInvalidOperationException()
        {
            Tools.CreateTestData(
                targetBets: new List<TargetBet>
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
                    }
                });

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
                    Target = 1,
                    Bet = "Bet"
                }
            };

            Assert.Throws<InvalidOperationException>(() =>
                _targetBetRepository.UpdateTargetBets(targetBets));
        }

        [Test]
        public void UpdateTargetBets_UpdatesTargetBets()
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

            Tools.CreateTestData(
                targetBets: targetBets);

            targetBets.ForEach(targetBet =>
            {
                targetBet.Bet = "Updated";
            });

            _targetBetRepository.UpdateTargetBets(targetBets);

            var updatedTargetBets = _context.TargetBet.ToList();
            Assert.AreEqual(2, updatedTargetBets.Count);

            updatedTargetBets.ForEach(targetBet =>
            {
                Assert.AreEqual("Updated", targetBet.Bet);
            });
        }

        [Test]
        public void GetTargetBets_ParticipatorParam_ReturnsParticipatorsBets()
        {
            var targetBets = new List<TargetBet>
            {
                new TargetBet
                {
                    Target = 1,
                    Participator = 1
                },
                new TargetBet
                {
                    Target = 2,
                    Participator = 1
                },
                new TargetBet
                {
                    Target = 1,
                    Participator = 2
                }
            };

            Tools.CreateTestData(targetBets: targetBets);

            Assert.AreEqual(
                2, 
                _targetBetRepository.GetTargetBets(participator: 1).Count);
        }
    }
}
