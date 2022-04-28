using System;
using System.Collections.Generic;
using PlutoDAO.Gov.Application.Options;
using PlutoDAO.Gov.Application.WhitelistedAssets;

namespace PlutoDAO.Gov.Application.Proposals.Responses
{
    public interface IProposalResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime Created { get; set; }
        public IEnumerable<WhitelistedAssetDto> WhitelistedAssets { get; set; }
        public IEnumerable<OptionDto> Options { get; set; }
        public string VotingResult { get; set; }
    }
}
