using PlutoDAO.Gov.Application.Proposals.Responses;
using PlutoDAO.Gov.Application.WhitelistedAssets;
using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Application.Proposals.Mappers
{
    public class ProposalMapper
    {
        public static IProposalResponse Map(Proposal proposal)
        {
            return new ProposalResponse
            {
             Name = proposal.Name,
             Description = proposal.Description,
             Creator = proposal.Creator,
             Deadline = proposal.Deadline,
             Created = proposal.Created,
             WhitelistedAssets = WhitelistedAssetMapper.Map(proposal.WhitelistedAssets)
            };
        }
    }
}
