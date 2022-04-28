using System.Threading.Tasks;
using PlutoDAO.Gov.Application.Proposals.Responses;
using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Application.Proposals
{
    public interface IProposalRepository
    {
        public Task<Proposal> GetProposal(string assetCode);
        public Task SaveProposal(Proposal proposal);
        public Task<ProposalIdentifier[]> GetProposalList();
        public Task<int> GetVotingResult(string assetCode);
    }
}
