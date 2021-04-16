using Betkeeper.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Betkeeper.Models
{
    public class CompetitionInvitation
    {
        [Key]
        public int InvitationId { get; set; }

        [Column("Competition")]
        public int CompetitionId { get; set; }

        public int UserId { get; set; }
    }

    public class CompetitionInvitationRepository
    {
        private readonly BetkeeperDataContext _context;

        public CompetitionInvitationRepository()
        {
            _context = new BetkeeperDataContext(Settings.OptionsBuilder);
        }

        /// <summary>
        /// Add invitations
        /// </summary>
        /// <param name="invitations"></param>
        public void AddInvitations(IEnumerable<CompetitionInvitation> invitations)
        {
            _context.CompetitionInvitation.AddRange(invitations);
            _context.SaveChanges();
        }

        /// <summary>
        /// Delete an invitation
        /// </summary>
        /// <param name="invitationId"></param>
        public void DeleteInvitation(int invitationId)
        {
            _context.CompetitionInvitation.Remove(
                _context.CompetitionInvitation.First(
                    invitation => invitation.InvitationId == invitationId));

            _context.SaveChanges();
        }

        /// <summary>
        /// Get invitations
        /// </summary>
        /// <param name="invitationId"></param>
        /// <param name="userId"></param>
        /// <param name="competitionId"></param>
        /// <returns></returns>
        public List<CompetitionInvitation> GetInvitations(
            int? invitationId = null, 
            int? userId = null,
            int? competitionId = null)
        {
            var query = _context.CompetitionInvitation.AsQueryable();

            if (invitationId != null)
            {
                query = query.Where(invitation => invitation.InvitationId == invitationId);
            }

            if (userId != null)
            {
                query = query.Where(invitation => invitation.UserId == userId);
            }

            if (competitionId != null)
            {
                query = query.Where(invitation => invitation.CompetitionId == competitionId);
            }

            return query.ToList();
        }
    }
}
