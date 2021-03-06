using System.Linq;
using PlutoDAO.Gov.Application.Options;
using PlutoDAO.Gov.Application.Proposals.Responses;
using PlutoDAO.Gov.Application.WhitelistedAssets;
using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Application.Proposals.Mappers
{
    public static class ProposalMapper
    {
        public static IProposalResponse Map(Proposal proposal, string votingResult)
        {
            return new ProposalResponse
            {
                Name = proposal.Name,
                Description = proposal.Description,
                Creator = proposal.Creator,
                Deadline = proposal.Deadline,
                Created = proposal.Created,
                WhitelistedAssets = proposal.WhitelistedAssets.Select(WhitelistedAssetMapper.Map),
                Options = proposal.Options.Select(option => (OptionDto)option),
                VotingResult = votingResult
            };
        }
    }
}
