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

        public ProposalService(IProposalRepository proposalRepository, DateTimeProvider dateTimeProvider)
        {
            _proposalRepository = proposalRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IProposalResponse> GetProposal(string assetCode)
        {
            var proposal = await _proposalRepository.GetProposal(assetCode);
            string voteResult = null;
            if (proposal == null)
                throw new ProposalNotFoundException($"Could not find proposal for {assetCode}", null,
                    "Proposal not found", "PROPOSAL_NOT_FOUND");

            if (proposal.Deadline < _dateTimeProvider.Now.Date)
            {
                const int indexShift = 1;
                var optionIndex = await _proposalRepository.GetVotingResult(assetCode);
                voteResult = optionIndex > 0 ? proposal.Options.ElementAt(optionIndex - indexShift).Name : "DRAW";
            }

            return ProposalMapper.Map(proposal, voteResult);
        }

        public async Task Save(IProposalRequest request)
        {
            var proposal = new Proposal(request.Name,
                request.Description,
                request.Creator,
                AssetHelper.GetWhitelistedAssets());

            await _proposalRepository.SaveProposal(proposal);
        }

        public async Task<IProposalIdentifier[]> GetList()
        {
            return await _proposalRepository.GetProposalList();
        }
    }
}
