using Betkeeper.Data;
using Betkeeper.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TestTools;

namespace Betkeeper.Test.Models
{
    public class ParticipatorTests
    {
        private ParticipatorRepository _participatorRepository;

        private BetkeeperDataContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = Tools.GetTestContext();
            _participatorRepository = new ParticipatorRepository();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Participator.RemoveRange(_context.Participator);
            _context.Competition.RemoveRange(_context.Competition);
            _context.SaveChanges();
        }

        [Test]
        public void GetParticipators_FiltersCorrectly()
        {
            var competitions = new List<Competition>
            {
                new Competition()
                {
                    JoinCode = "joincode",
                    Name = "Name",
                    CompetitionId = 1
                },
                new Competition()
                {
                    JoinCode = "joincode2",
                    Name = "Name2",
                    CompetitionId = 2
                }
            };

            var participators = new List<Participator>
            {
                new Participator
                {
                    Competition = 1,
                    UserId = 1
                },
                new Participator
                {
                    Competition = 2,
                    UserId = 1
                },
                new Participator
                {
                    Competition = 2,
                    UserId = 2
                },
                new Participator
                {
                    Competition = 1,
                    UserId = 2
                }
            };

            Tools.CreateTestData(competitions: competitions, participators: participators);

            Assert.AreEqual(4, _participatorRepository.GetParticipators().Count);
            Assert.AreEqual(2, _participatorRepository.GetParticipators(competitionId: 2).Count);
            Assert.AreEqual(2, _participatorRepository.GetParticipators(userId: 2).Count);
            Assert.AreEqual(1, _participatorRepository.GetParticipators(userId: 1, competitionId: 1).Count);
        }

        [Test]
        public void AddParticipator_CompetitionDoesNotExist_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _participatorRepository.AddParticipator(
                    userId: 1, competitionId: 1, Enums.CompetitionRole.Host));
        }

        [Test]
        public void AddParticipator_CompetitionExistst_ParticipatorAdded()
        {
            var competitions = new List<Competition>
            {
                new Competition()
                {
                    JoinCode = "joincode",
                    Name = "Name",
                    CompetitionId = 1
                }
            };

            Tools.CreateTestData(competitions: competitions);

            _participatorRepository.AddParticipator(
                userId: 1, competitionId: 1, Enums.CompetitionRole.Host);
 
            Assert.AreEqual(1, _context.Participator.ToList().Count);
        }
    }
}
