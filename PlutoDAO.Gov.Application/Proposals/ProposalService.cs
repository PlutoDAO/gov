using System.Threading.Tasks;
using PlutoDAO.Gov.Application.Exceptions;
using PlutoDAO.Gov.Application.Proposals.Mappers;
using PlutoDAO.Gov.Application.Proposals.Responses;

namespace PlutoDAO.Gov.Application.Proposals
{
    public class ProposalService
    {
        private readonly IProposalRepository _proposalRepository;

        public ProposalService(IProposalRepository proposalRepository)
        {
            _proposalRepository = proposalRepository;
        }
        
        public async Task<IProposalResponse> FindByAddress(string address)
        {
            var proposal = await _proposalRepository.FindProposal(address);

            if (proposal == null)
            {
                throw new ProposalNotFoundException($"Could not find proposal for {address}",null,
                    "Proposal not found", "PROPOSAL_NOT_FOUND");
            }

            return ProposalMapper.Map(proposal);
        }
    }
}
