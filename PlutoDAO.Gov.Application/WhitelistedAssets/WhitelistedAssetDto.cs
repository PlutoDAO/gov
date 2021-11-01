using System.Collections.Generic;
using PlutoDAO.Gov.Application.Assets;

namespace PlutoDAO.Gov.Application.WhitelistedAssets
{
    public class WhitelistedAssetDto : IWhitelistedAssetDto
    {
        public Dictionary<AssetDto, decimal> WhitelistedAsset { get; set; }
    }
}
