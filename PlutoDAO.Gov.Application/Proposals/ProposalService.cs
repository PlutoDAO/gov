using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlutoDAO.Gov.Application.Exceptions;
using PlutoDAO.Gov.Application.Proposals.Mappers;
using PlutoDAO.Gov.Application.Proposals.Requests;
using PlutoDAO.Gov.Application.Proposals.Responses;
using PlutoDAO.Gov.Application.Providers;
using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Application.Proposals
{
    public class ProposalService
    {
        private readonly IProposalRepository _proposalRepository;
        private readonly DateTimeProvider _dateTimeProvider;

        public ProposalService(IProposalRepository proposalRepository)
        public ProposalService(IProposalRepository proposalRepository, DateTimeProvider dateTimeProvider)
        {
            _proposalRepository = proposalRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IProposalResponse> GetProposal(string assetCode)
        {
            var proposal = await _proposalRepository.GetProposal(assetCode);

            if (proposal == null)
                throw new ProposalNotFoundException($"Could not find proposal for {assetCode}", null,
                    "Proposal not found", "PROPOSAL_NOT_FOUND");

            return ProposalMapper.Map(proposal);
        }

        public async Task Save(IProposalRequest request)
        {
            var proposal = new Proposal(request.Name,
                request.Description,
                request.Creator,
                new List<WhitelistedAsset>(request.WhitelistedAssets.Select(w => (WhitelistedAsset) w)));

            await _proposalRepository.SaveProposal(proposal);
        }

        public async Task<IProposalIdentifier[]> GetList()
        {
            return await _proposalRepository.GetProposalList();
        }
    }
}
