using System.Threading.Tasks;
using PlutoDAO.Gov.Application.Proposals.Responses;
using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Application.Proposals
{
    public interface IProposalRepository
    {
        public Task<Proposal> FindProposal(string address);
        public Task SaveProposal(Proposal proposal);
        public Task<Proposal[]> GetProposals();
        public Task<ProposalIdentifier[]> GetProposalList();
    }
}
