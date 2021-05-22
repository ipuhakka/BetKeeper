using Betkeeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Betkeeper.Actions
{
    public class CompetitionInvitationAction
    {
        /// <summary>
        /// Returns users invitationn (competition name and start time)
        /// </summary>
        /// <returns></returns>
        public static List<PrettyInvitation> GetUsersInvitations(int userId)
        {
            var invitations = new CompetitionInvitationRepository().GetInvitations(userId: userId);

            var competitions = new CompetitionRepository()
                .GetCompetitionsById(invitations.Select(invitation => invitation.CompetitionId).ToList());

            return competitions
                .Select(competition => new PrettyInvitation
                    {
                        CompetitionName = competition.Name,
                        StartTime = competition.StartTime,
                        InvitationId = invitations
                            .Single(invitations => invitations.CompetitionId == competition.CompetitionId)
                            .InvitationId
                    })
                .ToList();
        } 

        /// <summary>
        /// Invite users to competition
        /// </summary>
        /// <param name="competitionId"></param>
        /// <param name="hostUserId"></param>
        /// <param name="userIds"></param>
        public static void InviteUsers(int competitionId, int hostUserId, List<string> usernames)
        {
            var competition = new CompetitionAction().GetCompetition(competitionId);

            if (competition == null)
            {
                throw new ActionException(Enums.ActionResultType.NotFound, "Competition does not exist");
            }

            if (competition.State != Enums.CompetitionState.Open)
            {
                throw new ActionException(Enums.ActionResultType.Conflict, "Competition not open for invitations");
            }

            var participators = new ParticipatorRepository()
                .GetParticipators(competitionId: competitionId);

            if (participators.SingleOrDefault(participator => 
                participator.UserId == hostUserId && 
                participator.Role == Enums.CompetitionRole.Host) == null)
            {
                throw new ActionException(Enums.ActionResultType.Conflict, "Only host can invite to competition");
            }

            var invitations = new CompetitionInvitationRepository().GetInvitations(competitionId: competitionId);

            var users = new UserRepository().GetUsersByName(usernames)
                .Where(user => 
                    participators.All(participator => participator.UserId != user.UserId) &&
                    invitations.All(invitation => invitation.UserId != user.UserId));

            var invitationsToAdd = users.Select(user => new CompetitionInvitation
            {
                CompetitionId = competitionId,
                UserId = user.UserId
            });

            new CompetitionInvitationRepository().AddInvitations(invitationsToAdd);
        }

        /// <summary>
        /// Accept an invitation for user and join user to competition
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="invitationId"></param>
        public static void AcceptInvitation(int userId, int invitationId)
        {
            var invitation = new CompetitionInvitationRepository()
                .GetInvitations(invitationId: invitationId, userId: userId)
                .SingleOrDefault();

            if (invitation == null)
            {
                throw new ActionException(Enums.ActionResultType.Conflict, "Cannot accept invitation of another user");
            }

            var competition = new CompetitionRepository().GetCompetition(competitionId: invitation.CompetitionId);

            if (competition == null || competition.State != Enums.CompetitionState.Open)
            {
                new CompetitionInvitationRepository().DeleteInvitation(invitationId);
                throw new ActionException(Enums.ActionResultType.Conflict, "Competition not open");
            }

            new ParticipatorRepository().AddParticipator(invitation.UserId, invitation.CompetitionId, Enums.CompetitionRole.Participator);
            new CompetitionInvitationRepository().DeleteInvitation(invitationId);
        }

        /// <summary>
        /// Decline an invitation to competition
        /// </summary>
        /// <param name="userId"
        /// <param name="invitationId"></param>
        public static void DeclineInvitation(int userId, int invitationId)
        {
            var invitation = new CompetitionInvitationRepository()
                .GetInvitations(invitationId: invitationId, userId: userId)
                .SingleOrDefault();

            if (invitation == null)
            {
                throw new ActionException(Enums.ActionResultType.Conflict, "Cannot accept invitation of another user");
            }

            new CompetitionInvitationRepository().DeleteInvitation(invitationId);
        }
    }

    public class PrettyInvitation
    {
        public DateTime StartTime { get; set; }

        public string CompetitionName { get; set; }

        public int InvitationId { get; set; }
    }
}
