using System;
using PlutoDAO.Gov.Application.WhitelistedAssets;

namespace PlutoDAO.Gov.Application.Proposals.Responses
{
    public class ProposalResponse : IProposalResponse
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public string Creator {get; set;}
        public DateTime Deadline {get; set;}
        public DateTime Created {get; set;}
        public WhitelistedAssetDto WhitelistedAssets {get; set;}
    }
}
