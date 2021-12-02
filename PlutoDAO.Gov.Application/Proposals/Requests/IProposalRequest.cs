using System.Collections.Generic;
using PlutoDAO.Gov.Application.WhitelistedAssets;

namespace PlutoDAO.Gov.Application.Proposals.Requests
{
    public interface IProposalRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public string Deadline { get; set; }
        public IEnumerable<WhitelistedAssetDto> WhitelistedAssets { get; set; }
    }
}
