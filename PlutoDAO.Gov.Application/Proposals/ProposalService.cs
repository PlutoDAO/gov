using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlutoDAO.Gov.Application.Exceptions;
using PlutoDAO.Gov.Application.Proposals.Mappers;
using PlutoDAO.Gov.Application.Proposals.Requests;
using PlutoDAO.Gov.Application.Proposals.Responses;
using PlutoDAO.Gov.Domain;

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
                throw new ProposalNotFoundException($"Could not find proposal for {address}", null,
                    "Proposal not found", "PROPOSAL_NOT_FOUND");

            return ProposalMapper.Map(proposal);
        }

        public async Task Save(IProposalRequest request)
        {
            var proposal = new Proposal(request.Name,
                request.Description,
                request.Creator,
                DateTime.Parse(request.Deadline),
                DateTime.Now,
                new List<WhitelistedAsset>(request.WhitelistedAssets.Select(w => (WhitelistedAsset) w)));
            await _proposalRepository.SaveProposal(proposal);
        }

        public async Task<IProposalResponse[]> GetAll()
        {
            var proposals = await _proposalRepository.GetProposals();
            return proposals.Select(ProposalMapper.Map).ToArray();
        }
    }
}
