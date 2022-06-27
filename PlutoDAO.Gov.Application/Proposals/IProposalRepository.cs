using PlutoDAO.Gov.Application.Proposals.Responses;
using PlutoDAO.Gov.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlutoDAO.Gov.Application.Proposals
{
    public interface IProposalRepository
    {
        public Task<Proposal> GetProposal(string assetCode);
        public Task SaveProposal(Proposal proposal);
        public Task<List<ProposalIdentifier>> GetProposalList();
        public Task<int> GetVotingResult(string assetCode);
    }
}
