using System.Threading.Tasks;
using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Application.Votes
{
    public interface IVoteRepository
    {
        public Task<string> GetVoteIntent(ValidatedVote validatedVote, Proposal proposal, string proposalId);
        public Task Vote(ValidatedVote validatedVote, Proposal proposal, string proposalId, string voterPrivateKey);
    }
}
