using Betkeeper.Actions;
using Betkeeper.Data;
using Betkeeper.Enums;
using Betkeeper.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TestTools;

namespace Betkeeper.Test.Actions
{
    [TestFixture]
    public class CompetitionInvitationActionTests
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
            _context.CompetitionInvitation.RemoveRange(_context.CompetitionInvitation);
            _context.User.RemoveRange(_context.User);
            _context.Competition.RemoveRange(_context.Competition);
            _context.Participator.RemoveRange(_context.Participator);
            _context.SaveChanges();
        }

        [Test]
        public void GetUsersInvitations_ReturnsUsersInvitations()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    Name = "test1",
                    StartTime = DateTime.Now.AddDays(1)
                },
                new Competition
                {
                    CompetitionId = 2,
                    Name = "test2",
                    StartTime = DateTime.Now.AddDays(1)
                },
                new Competition
                {
                    CompetitionId = 3,
                    Name = "test3",
                    StartTime = DateTime.Now.AddDays(1)
                }
            };

            var invitations = new List<CompetitionInvitation>
            {
                new CompetitionInvitation
                {
                    UserId = 1,
                    CompetitionId = 1
                },
                new CompetitionInvitation
                {
                    UserId = 2,
                    CompetitionId = 2
                },
                new CompetitionInvitation
                {
                    UserId = 1,
                    CompetitionId = 3
                }
            };

            Tools.CreateTestData(invitations: invitations, competitions: competitions);

            var results = new CompetitionInvitationAction().GetUsersInvitations(1);

            Assert.AreEqual(2, results.Count);
        }

        [Test]
        public void InviteUsers_CompetitionDoesNotExist_ThrowsActionException()
        {
            try
            {
                new CompetitionInvitationAction().InviteUsers(1, 2, new List<string> { "testi" });
                Assert.Fail();
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionResultType.NotFound, e.ActionExceptionType);
            }
        }

        [Test]
        public void InviteUsers_CompetitionNotStarted_ThrowsActionException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.Now.AddDays(-1)
                }
            };

            var users = new List<User>
            {
                new User
                {
                    Username = "testi"
                }
            };

            Tools.CreateTestData(competitions: competitions, users: users);

            try
            {
                new CompetitionInvitationAction().InviteUsers(1, 2, new List<string> { "testi" });
                Assert.Fail();
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionResultType.Conflict, e.ActionExceptionType);
            }
        }

        [Test]
        public void InviteUsers_UserNotCompetitionHost_ThrowsActionException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.Now.AddDays(-1)
                }
            };

            var users = new List<User>
            {
                new User
                {
                    Username = "testi"
                }
            };

            Tools.CreateTestData(competitions: competitions, users: users);

            try
            {
                new CompetitionInvitationAction().InviteUsers(1, 2, new List<string> { "testi" });
                Assert.Fail();
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionResultType.Conflict, e.ActionExceptionType);
            }
        }

        [Test]
        public void InviteUsers_AddsInvitationsForUsersThatExistAndDontHaveAnInvitationAndAreNotInCompetition()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.Now.AddDays(1)
                }
            };

            var users = new List<User>
            {
                new User
                {
                    Username = "testi",
                    UserId = 1
                },
                new User
                {
                    Username = "IsInCompetition",
                    UserId = 3
                },
                new User
                {
                    Username = "IsInvited",
                    UserId = 4
                }
            };

            var participators = new List<Participator>
            {
                new Participator
                {
                    UserId = 2,
                    Competition = 1,
                    Role = CompetitionRole.Host
                },
                new Participator
                {
                    UserId = 3,
                    Competition = 1,
                    Role = CompetitionRole.Participator
                }
            };

            var invitations = new List<CompetitionInvitation>
            {
                new CompetitionInvitation
                {
                    CompetitionId = 1,
                    UserId = 4
                }
            };

            Tools.CreateTestData(
                competitions: competitions, 
                users: users, 
                participators: participators,
                invitations: invitations);

            new CompetitionInvitationAction().InviteUsers(1, 2, new List<string> { "testi", "notexistingUser", "IsInvited", "IsInCompetition" });

            var results = _context.CompetitionInvitation.ToList();
            // One already exists
            Assert.AreEqual(2, results.Count);
        }

        [Test]
        public void AcceptInvitation_NotSameUser_ThrowsActionException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.Now.AddDays(-1)
                }
            };

            var users = new List<User>
            {
                new User
                {
                    Username = "testi",
                    UserId = 1
                }
            };

            var invitations = new List<CompetitionInvitation>
            {
                new CompetitionInvitation
                {
                    UserId = 1,
                    CompetitionId = 1,
                    InvitationId = 1
                },
                new CompetitionInvitation
                {
                    UserId = 2,
                    CompetitionId = 1,
                    InvitationId = 2
                }
            };

            Tools.CreateTestData(competitions: competitions, users: users, invitations: invitations);

            try
            {
                new CompetitionInvitationAction().AcceptInvitation(2, 1);
                Assert.Fail();
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionResultType.Conflict, e.ActionExceptionType);
            }
        }

        [Test]
        public void AcceptInvitation_CompetitionNotOpen_InviteDeleted_ThrowsActionException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.Now.AddDays(-1)
                }
            };

            var invitations = new List<CompetitionInvitation>
            {
                new CompetitionInvitation
                {
                    UserId = 1,
                    CompetitionId = 1,
                    InvitationId = 1
                }
            };

            Tools.CreateTestData(competitions: competitions, invitations: invitations);

            try
            {
                new CompetitionInvitationAction().AcceptInvitation(1, 1);
                Assert.Fail();
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionResultType.Conflict, e.ActionExceptionType);
                Assert.AreEqual("Competition not open", e.ErrorMessage);

                Assert.AreEqual(0, _context.CompetitionInvitation.Count());
            }
        }

        [Test]
        public void AcceptInvitation_OwnInvitation_Succeeds()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.Now.AddDays(1)
                }
            };

            var users = new List<User>
            {
                new User
                {
                    Username = "testi",
                    UserId = 1
                }
            };

            var invitations = new List<CompetitionInvitation>
            {
                new CompetitionInvitation
                {
                    UserId = 1,
                    CompetitionId = 1,
                    InvitationId = 1
                },
                new CompetitionInvitation
                {
                    UserId = 2,
                    CompetitionId = 1,
                    InvitationId = 2
                }
            };

            Tools.CreateTestData(competitions: competitions, users: users, invitations: invitations);

            new CompetitionInvitationAction().AcceptInvitation(2, 2);

            Assert.AreEqual(1, _context.Participator.ToList().Count, "Participator count");
            // Check that invitation was deleted
            Assert.AreEqual(1, _context.CompetitionInvitation.ToList().Count, "Invitation count");
        }

        [Test]
        public void DeclineInvitation_NotSameUser_ThrowsActionException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.Now.AddDays(-1)
                }
            };

            var users = new List<User>
            {
                new User
                {
                    Username = "testi",
                    UserId = 1
                }
            };

            var invitations = new List<CompetitionInvitation>
            {
                new CompetitionInvitation
                {
                    UserId = 1,
                    CompetitionId = 1,
                    InvitationId = 1
                },
                new CompetitionInvitation
                {
                    UserId = 2,
                    CompetitionId = 1,
                    InvitationId = 2
                }
            };

            Tools.CreateTestData(competitions: competitions, users: users, invitations: invitations);

            try
            {
                new CompetitionInvitationAction().DeclineInvitation(2, 1);
                Assert.Fail();
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionResultType.Conflict, e.ActionExceptionType);
            }
        }

        [Test]
        public void DeclineInvitation_OwnInvitation_Succeeds()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.Now.AddDays(-1)
                }
            };

            var users = new List<User>
            {
                new User
                {
                    Username = "testi",
                    UserId = 1
                }
            };

            var invitations = new List<CompetitionInvitation>
            {
                new CompetitionInvitation
                {
                    UserId = 1,
                    CompetitionId = 1,
                    InvitationId = 1
                },
                new CompetitionInvitation
                {
                    UserId = 2,
                    CompetitionId = 1,
                    InvitationId = 2
                }
            };

            Tools.CreateTestData(competitions: competitions, users: users, invitations: invitations);

            new CompetitionInvitationAction().DeclineInvitation(2, 2);

            // Check that invitation was deleted
            Assert.AreEqual(1, _context.CompetitionInvitation.ToList().Count, "Invitation count");

            // Check that no participators were created
            Assert.AreEqual(0, _context.Participator.ToList().Count, "Participator count");
        }
    }
}
